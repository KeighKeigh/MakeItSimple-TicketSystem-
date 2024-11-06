﻿using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest
{
    public partial class AddRequestConcern
    {

        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;
            private readonly TransformUrl _url;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernList = new int();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var userId = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.UserId);

                var locationExist = await _context.Locations
                    .FirstOrDefaultAsync(l => l.LocationCode == command.Location_Code, cancellationToken);
                
                var validationResult = await ValidateEntities(userId,locationExist ,command, cancellationToken);
                if (validationResult is not null)
                    return validationResult;

                var requestConcernIdExist = await _context.RequestConcerns
                    .Include(x => x.User)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);

                if (requestConcernIdExist is not null)
                {

                    var ticketConcernExist = await _context.TicketConcerns
                        .FirstOrDefaultAsync(x => x.RequestConcernId == requestConcernIdExist.Id, cancellationToken);

                    await UpdateRequest(requestConcernIdExist,locationExist ,ticketConcernExist,command, cancellationToken);

                    ticketConcernList = ticketConcernExist.Id;
                }
                else
                {

                    var addRequestConcern = await AddRequestConcern(userId,locationExist ,command, cancellationToken);
                    var addTicketConcern = await AddTicketConcern(addRequestConcern, command, cancellationToken);

                    ticketConcernList = addTicketConcern.Id;

                    await AddTicketHistory(userId, addTicketConcern, command, cancellationToken);
                    await AddNewTicketTransactionNotification(userDetails, addRequestConcern, command, cancellationToken);

                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0)
                {
                   var attachment =  await AttachmentHandler(command, ticketConcernList, cancellationToken);
                    if(attachment is not null)
                        return attachment;
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }


            private async Task<RequestConcern> UpdateRequest(RequestConcern requestConcernIdExist,Location location,TicketConcern ticketConcernExist, AddRequestConcernCommand command , CancellationToken cancellationToken)
            {
                
                bool isChange = false;

                if (requestConcernIdExist.Concern != command.Concern)
                {
                    requestConcernIdExist.Concern = command.Concern;
                    isChange = true;
                }

                if (requestConcernIdExist.CompanyId != command.CompanyId)
                {
                    requestConcernIdExist.CompanyId = command.CompanyId;
                    isChange = true;
                }
                if (requestConcernIdExist.BusinessUnitId != command.BusinessUnitId)
                {
                    requestConcernIdExist.BusinessUnitId = command.BusinessUnitId;
                    isChange = true;
                }
                if (requestConcernIdExist.BusinessUnitId != command.BusinessUnitId)
                {
                    requestConcernIdExist.BusinessUnitId = command.BusinessUnitId;
                    isChange = true;
                }
                if (requestConcernIdExist.LocationId != location.Id)
                {
                    requestConcernIdExist.LocationId = location.Id;
                    isChange = true;
                }
                if (requestConcernIdExist.ChannelId != command.ChannelId)
                {
                    requestConcernIdExist.ChannelId = command.ChannelId;
                    isChange = true;
                }
                if (requestConcernIdExist.CategoryId != command.CategoryId)
                {
                    requestConcernIdExist.SubCategoryId = command.SubCategoryId;
                    isChange = true;
                }
                if (requestConcernIdExist.ContactNumber != command.Contact_Number)
                {
                    requestConcernIdExist.ContactNumber = command.Contact_Number;
                    isChange = true;
                }
                if (requestConcernIdExist.RequestType != command.Request_Type)
                {
                    requestConcernIdExist.RequestType = command.Request_Type;
                    isChange = true;
                }

                if (requestConcernIdExist.DateNeeded != command.DateNeeded)
                {
                    requestConcernIdExist.DateNeeded = command.DateNeeded;
                    isChange = true;
                }

                if(requestConcernIdExist.BackJobId != command.BackJobId)
                {
                    requestConcernIdExist.BackJobId = command.BackJobId;
                    isChange = true;    
                }

                if (isChange)
                {
                    requestConcernIdExist.ModifiedBy = command.Modified_By;
                    requestConcernIdExist.UpdatedAt = DateTime.Now;
                    ticketConcernExist.UpdatedAt = DateTime.Now;
                }

                return requestConcernIdExist;

            }

            private async Task<Result?> AttachmentHandler(AddRequestConcernCommand command, int ticketConcern, CancellationToken cancellationToken)
            {

                foreach (var attachments in command.RequestAttachmentsFiles.Where(a => a.Attachment.Length > 0))
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
                            TicketConcernId = ticketConcern,
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

            private async Task<Result?> ValidateEntities(User user,Location location,AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                if (user is null)
                    return Result.Failure(UserError.UserNotExist());

                var companyExist = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == command.CompanyId, cancellationToken);

                if (companyExist is null)
                    return Result.Failure(TicketRequestError.CompanyNotExist());

                var businessUnitExist = await _context.BusinessUnits
                    .FirstOrDefaultAsync(c => c.Id == command.BusinessUnitId, cancellationToken);

                if (businessUnitExist is null)
                    return Result.Failure(TicketRequestError.BusinessUnitNotExist());

                var departmentExist = await _context.Departments
                    .FirstOrDefaultAsync(c => c.Id == command.DepartmentId, cancellationToken);

                if (departmentExist is null)
                    return Result.Failure(TicketRequestError.DepartmentNotExist());

                var unitExist = await _context.Units
                    .FirstOrDefaultAsync(c => c.Id == command.UnitId, cancellationToken);

                if (unitExist is null)
                    return Result.Failure(TicketRequestError.UnitNotExist());

                var subUnitExist = await _context.SubUnits
                    .FirstOrDefaultAsync(c => c.Id == command.SubUnitId, cancellationToken);

                if (subUnitExist is null)
                    return Result.Failure(TicketRequestError.SubUnitNotExist());


                if (location is null)
                    return Result.Failure(TicketRequestError.LocationNotExist());

                var categoryExist = await _context.Categories
                  .FirstOrDefaultAsync(c => c.Id == command.CategoryId, cancellationToken);

                var channelExist = await _context.Channels
                  .FirstOrDefaultAsync(c => c.Id == command.ChannelId, cancellationToken);

                if (categoryExist is null)
                    return Result.Failure(TicketRequestError.CategoryNotExist());

                var subCategoryExist = await _context.SubCategories
                    .FirstOrDefaultAsync(c => c.Id == command.SubCategoryId, cancellationToken);

                if (subCategoryExist is null)
                    return Result.Failure(TicketRequestError.SubUnitNotExist());

                return null;
            }

            private async Task<RequestConcern> AddRequestConcern(User user,Location location ,AddRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var addRequestConcern = new RequestConcern
                {
                    UserId = user.Id,
                    Concern = command.Concern,
                    AddedBy = command.Added_By,
                    ConcernStatus = TicketingConString.ForApprovalTicket,
                    CompanyId = command.CompanyId,
                    BusinessUnitId = command.BusinessUnitId,
                    DepartmentId = command.DepartmentId,
                    UnitId = command.UnitId,
                    SubUnitId = command.SubUnitId,
                    LocationId = location.Id,
                    DateNeeded = command.DateNeeded,
                    ChannelId = command.ChannelId,
                    CategoryId = command.CategoryId,
                    SubCategoryId = command.SubCategoryId,
                    Notes = command.Notes,
                    IsDone = false,
                    ContactNumber = command.Contact_Number,
                    RequestType = command.Request_Type,
                    BackJobId = command.BackJobId,

                };

                await _context.RequestConcerns.AddAsync(addRequestConcern);
                await _context.SaveChangesAsync(cancellationToken);

                return addRequestConcern;

            }

            private async Task<TicketConcern> AddTicketConcern(RequestConcern requestConcern, AddRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var addTicketConcern = new TicketConcern
                {
                    RequestConcernId = requestConcern.Id,
                    RequestorBy = command.UserId,
                    IsApprove = false,
                    AddedBy = command.Added_By,
                    ConcernStatus = requestConcern.ConcernStatus,
                    IsAssigned = false,

                };

                await _context.TicketConcerns.AddAsync(addTicketConcern);
                await _context.SaveChangesAsync(cancellationToken);

                return addTicketConcern;
            }

            private async Task<TicketHistory> AddTicketHistory(User user, TicketConcern ticketConcern, AddRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Request,
                    Status = $"{TicketingConString.RequestCreated} {user.Fullname}"
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                return addTicketHistory;

            }

            private async Task<TicketTransactionNotification> AddNewTicketTransactionNotification(User user, RequestConcern requestConcern, AddRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var userReceiver = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == requestConcern.User.BusinessUnitId);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"New request concern number {requestConcern.Id} has received",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = userReceiver.UserId.Value,
                    Modules = PathConString.ReceiverConcerns,
                    PathId = requestConcern.Id

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;

            }

        }
    }
}
