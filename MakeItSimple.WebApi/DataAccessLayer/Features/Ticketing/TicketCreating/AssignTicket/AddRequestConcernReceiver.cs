using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket
{
    public partial class AddRequestConcernReceiver
    {

        public class Handler : IRequestHandler<AddRequestConcernReceiverCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var requestConcernId = new int();
                var ticketCategoryList = new List<int>();
                var ticketSubCategoryList = new List<int>();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Modified_By, cancellationToken);

                var allUserList = await _context.UserRoles
                    .ToListAsync();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var issueHandlerPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.IssueHandler))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var validation = await ValidationHandler(command, cancellationToken);
                if(validation is not null) 
                    return validation;
                
                var upsertConcern = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId,cancellationToken);

                if (upsertConcern is not null)
                {
                    if (upsertConcern.IsActive is false)
                        return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                    requestConcernId = upsertConcern.Id;
                    await AssignTicket(upsertConcern,command,cancellationToken);

                    await TransactionNotification(upsertConcern, userDetails, command, cancellationToken);

                    await RequestorTransactionNotification(upsertConcern, userDetails, command, cancellationToken);

                }
                else
                {

                    var requestorDetails = await _context.Users
                         .FirstOrDefaultAsync(r => r.Id == command.Requestor_By);

                    var createRequestConcern = await CreateRequestConcern(requestorDetails,command,cancellationToken);

                    requestConcernId = createRequestConcern.Id;

                    var createTicketConcern = await CreateTicketConcern(createRequestConcern, requestorDetails, command, cancellationToken);

                    upsertConcern = createTicketConcern;

                    await TicketingHistory(createTicketConcern,userDetails, command, cancellationToken);
                    await ReceiverTransactionNotification(createTicketConcern,userDetails, command, cancellationToken);
                }

                foreach (var category in command.RequestorTicketCategories)
                {
                    var ticketCategoryExist = await _context.TicketCategories
                        .FirstOrDefaultAsync(t => t.Id == category.TicketCategoryId, cancellationToken);

                    if (ticketCategoryExist is not null)
                    {
                        ticketCategoryList.Add(category.TicketCategoryId.Value);
                    }
                    else
                    {
                        await CreateTicketCategory(requestConcernId, category, cancellationToken);
                    }

                }

                foreach (var subCategory in command.RequestorTicketSubCategories)
                {
                    var ticketSubCategoryExist = await _context.TicketSubCategories
                        .FirstOrDefaultAsync(t => t.Id == subCategory.TicketSubCategoryId, cancellationToken);

                    if (ticketSubCategoryExist is not null)
                    {
                        ticketSubCategoryList.Add(subCategory.TicketSubCategoryId.Value);
                    }
                    else
                    {
                        await CreateSubTicketCategory(requestConcernId, subCategory, cancellationToken);
                    }
                     
                }

                //if (ticketCategoryList.Any())
                    await RemoveTicketCategory(requestConcernId, ticketCategoryList, cancellationToken);

                //if (ticketSubCategoryList.Any())
                    await RemoveTicketSubCategory(requestConcernId, ticketSubCategoryList, cancellationToken);

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.ConcernAttachments.Count(x => x.Attachment != null) > 0)
                {
                    await AttachmentHandler(upsertConcern,command, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
  
            private async Task<Result?> ValidationHandler(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;


                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                var channelExist = await _context.Channels
                  .FirstOrDefaultAsync(c => c.Id == command.ChannelId, cancellationToken);

                if (channelExist is null)
                    return Result.Failure(TicketRequestError.ChannelNotExist());

                foreach (var category in command.RequestorTicketCategories)
                {
                    var ticketCategoryExist = await _context.Categories
                      .FirstOrDefaultAsync(c => c.Id == category.CategoryId, cancellationToken);

                    if (ticketCategoryExist is null)
                        return Result.Failure(TicketRequestError.CategoryNotExist());

                }

                foreach (var subCategory in command.RequestorTicketSubCategories)
                {
                    var ticketSubCategoryExist = await _context.SubCategories
                        .FirstOrDefaultAsync(c => c.Id == subCategory.SubCategoryId, cancellationToken);

                    if (ticketSubCategoryExist is null)
                        return Result.Failure(TicketRequestError.SubCategoryNotExist());

                }

                if (dateToday > command.Target_Date)
                    return Result.Failure(TicketRequestError.DateTimeInvalid());
                
                return null;
            }

            private async Task<TicketConcern> AssignTicket(TicketConcern ticketConcern , AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {

                bool hasChanged = false;

                if (ticketConcern.UserId != command.UserId)
                {
                    ticketConcern.UserId = command.UserId;
                    hasChanged = true;
                }

                if (ticketConcern.TargetDate != command.Target_Date)
                {
                    ticketConcern.TargetDate = command.Target_Date;
                    hasChanged = true;
                }


                if (ticketConcern.RequestConcernId is not null)
                {
                    var requestConcern = await _context.RequestConcerns
                        .FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestConcernId, cancellationToken);

                    if (requestConcern.ChannelId != command.ChannelId)
                    {
                        requestConcern.ChannelId = command.ChannelId;
                        hasChanged = true;
                    }


                    if(hasChanged)
                    {
                        requestConcern.ModifiedBy = command.Modified_By;
                        requestConcern.UpdatedAt = DateTime.Now;
                    }

                    requestConcern.Remarks = null;
                }

                if (hasChanged)
                {
                    ticketConcern.ModifiedBy = command.Modified_By;
                    ticketConcern.UpdatedAt = DateTime.Now;
                    ticketConcern.IsAssigned = true;

                }

                return ticketConcern;

            }

            private async Task<TicketTransactionNotification> TransactionNotification(TicketConcern ticketConcern, User user,AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} has been assigned",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;
            }

            private async Task<TicketTransactionNotification> RequestorTransactionNotification(TicketConcern ticketConcern, User user, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionOngoing = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.RequestConcernId} is now ongoing",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketConcern.RequestConcern.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.Ongoing,
                    PathId = ticketConcern.RequestConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionOngoing);

                return addNewTicketTransactionOngoing;
            }

            private async Task<Result?> AttachmentHandler(TicketConcern ticketConcern, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken )
            {
                foreach (var attachments in command.ConcernAttachments.Where(a => a.Attachment.Length > 0))
                {

                    if (attachments.Attachment.Length > 10 * 1024 * 1024)
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                    }

                    var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                    var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                    if (extension == null || !allowedFileTypes.Contains(extension))
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentType());
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                    var ticketAttachment = await _context.TicketAttachments
                        .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                    if (ticketAttachment != null)
                    {
                        ticketAttachment.Attachment = filePath;
                        ticketAttachment.FileName = attachments.Attachment.FileName;
                        ticketAttachment.FileSize = attachments.Attachment.Length;
                        ticketAttachment.UpdatedAt = DateTime.Now;

                    }
                    else
                    {
                        var addAttachment = new TicketAttachment
                        {
                            TicketConcernId = ticketConcern.Id,
                            Attachment = filePath,
                            FileName = attachments.Attachment.FileName,
                            FileSize = attachments.Attachment.Length,
                            AddedBy = command.Added_By,
                        };

                        await _context.TicketAttachments.AddAsync(addAttachment);

                    }

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachments.Attachment.CopyToAsync(stream);
                    }
                }

                return null;
            }

            private async Task<RequestConcern> CreateRequestConcern(User user, AddRequestConcernReceiverCommand command,CancellationToken cancellationToken)
            {
                var addRequestConcern = new RequestConcern
                {
                    UserId = command.Requestor_By,
                    Concern = command.Concern,
                    AddedBy = command.Added_By,
                    ConcernStatus = TicketingConString.CurrentlyFixing,
                    CompanyId = user.CompanyId,
                    BusinessUnitId = user.BusinessUnitId,
                    DepartmentId = user.DepartmentId,
                    UnitId = user.UnitId,
                    SubUnitId = user.SubUnitId,
                    LocationId = user.LocationId,
                    ChannelId = command.ChannelId,
                    DateNeeded = command.DateNeeded,
                    ContactNumber = command.Contact_Number,
                    RequestType = command.Request_Type,
                    BackJobId = command.BackJobId,
                    Notes = command.Notes,
                    IsDone = false,

                };

                await _context.RequestConcerns.AddAsync(addRequestConcern);
                await _context.SaveChangesAsync(cancellationToken);

                return addRequestConcern;

            }

            private async Task<TicketConcern> CreateTicketConcern(RequestConcern requestConcern, User user, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addTicketConcern = new TicketConcern
                {
                    RequestConcernId = requestConcern.Id,
                    TargetDate = command.Target_Date,
                    UserId = command.UserId,
                    RequestorBy = command.Requestor_By,
                    IsApprove = true,
                    AddedBy = command.Added_By,
                    ConcernStatus = requestConcern.ConcernStatus,
                    IsAssigned = true,
                    ApprovedBy = command.Added_By,
                    ApprovedAt = DateTime.Now,

                };

                await _context.TicketConcerns.AddAsync(addTicketConcern);
                await _context.SaveChangesAsync(cancellationToken);

                return addTicketConcern;

            }

            private async Task TicketingHistory(TicketConcern ticketConcern, User user,AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addRequestTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Request,
                    Status = $"{TicketingConString.ConcernCreated} {user.Fullname}"
                };

                await _context.TicketHistories.AddAsync(addRequestTicketHistory, cancellationToken);


                var asignedTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ConcernAssign,
                    Status = $"{TicketingConString.RequestAssign} {ticketConcern.User.Fullname}"
                };

                await _context.TicketHistories.AddAsync(asignedTicketHistory, cancellationToken);

            }

            private async Task ReceiverTransactionNotification(TicketConcern ticketConcern, User user, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} has been assigned",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                var addNewTicketTransactionOngoing = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.RequestConcernId} is now ongoing",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.Requestor_By.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.Ongoing,
                    PathId = ticketConcern.RequestConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionOngoing);

            }

            private async Task CreateTicketCategory(int requestConcernId, AddRequestConcernReceiverCommand.RequestorTicketCategory category, CancellationToken cancellationToken)
            {

                var addTicketCategory = new TicketCategory
                {
                    RequestConcernId = requestConcernId,
                    CategoryId = category.CategoryId.Value,

                };

                await _context.TicketCategories.AddAsync(addTicketCategory);

            }

            private async Task CreateSubTicketCategory(int requestConcernId, AddRequestConcernReceiverCommand.RequestorTicketSubCategory subCategory, CancellationToken cancellationToken)
            {
                var addTicketSubCategory = new TicketSubCategory
                {
                    RequestConcernId = requestConcernId,
                    SubCategoryId = subCategory.SubCategoryId.Value,

                };

                await _context.TicketSubCategories.AddAsync(addTicketSubCategory);

            }

            private async Task RemoveTicketCategory(int requestConcernId, List<int> ticketCategoryList, CancellationToken cancellationToken)
            {
                var allTicketCategory = await _context.TicketCategories
                    .Where(r => r.RequestConcernId == requestConcernId)
                    .Select(a => new
                    {
                        a.Id,

                    }).ToListAsync();

                var removeTicketCategory = allTicketCategory
                    .Where(r => !ticketCategoryList.Contains(r.Id));

                foreach (var remove in removeTicketCategory)
                {
                    var ticketCategoryExist = await _context.TicketCategories
                        .FirstOrDefaultAsync(t => t.Id == remove.Id, cancellationToken);

                    _context.TicketCategories.Remove(ticketCategoryExist);

                }
            }

            private async Task RemoveTicketSubCategory(int requestConcernId, List<int> ticketSubCategoryList, CancellationToken cancellationToken)
            {
                var allSubTicketCategory = await _context.TicketSubCategories
                    .Where(r => r.RequestConcernId == requestConcernId)
                    .Select(a => new
                    {
                        a.Id,

                    }).ToListAsync();

                var removeSubTicketCategory = allSubTicketCategory
                    .Where(r => !ticketSubCategoryList.Contains(r.Id));

                foreach (var remove in removeSubTicketCategory)
                {
                    var ticketSubCategoryExist = await _context.TicketSubCategories
                        .FirstOrDefaultAsync(t => t.Id == remove.Id, cancellationToken);

                    _context.TicketSubCategories.Remove(ticketSubCategoryExist);
                }

            }

        }
    }
}
