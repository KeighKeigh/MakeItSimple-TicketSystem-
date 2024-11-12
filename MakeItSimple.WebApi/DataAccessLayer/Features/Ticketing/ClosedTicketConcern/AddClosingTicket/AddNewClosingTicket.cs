using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.AddClosingTicket
{

    public class Handler : IRequestHandler<AddNewClosingTicketCommand, Result>
    {
        private readonly MisDbContext _context;

        public Handler(MisDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {

            var userDetails = await _context.Users
               .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

            var ticketConcernExist = await _context.TicketConcerns
                .Include(x => x.User)
                .Include(x => x.RequestorByUser)
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

            if (ticketConcernExist is null)       
                return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());
            

            var closingTicketExist = await _context.ClosingTickets
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.RequestorByUser)
                .FirstOrDefaultAsync(x => x.Id == command.ClosingTicketId);

            if (closingTicketExist is not null)
            {
                var approver = await _context.ApproverTicketings
                    .Where(a => a.ClosingTicketId == closingTicketExist.Id && a.IsApprove != null)
                    .FirstOrDefaultAsync();

                if (approver is not null)
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());

                await UpdateClosingTicket(closingTicketExist,command,cancellationToken);

            }
            else
            {

                var approverList = await _context.Approvers
                    .Include(x => x.User)
                    .Where(x => x.SubUnitId == ticketConcernExist.User.SubUnitId)
                    .ToListAsync();

                var validation = await ValidationHandler(approverList, command, cancellationToken);
                if (validation is not null)
                    return validation;
                
                var approverUser = approverList
                    .First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                var newClosingTicket = await AddClosingTicket(approverUser, ticketConcernExist, command, cancellationToken);

                closingTicketExist = newClosingTicket;

                foreach (var approver in approverList)
                {
                    await AddApproverTicketing(ticketConcernExist, closingTicketExist, approver, command, cancellationToken);
                }

                await AddClosingHistory(ticketConcernExist, command, cancellationToken);

                foreach (var approver in approverList)
                {
                    await AddApproverHistory(ticketConcernExist, approver, command, cancellationToken);
                }

                await AddConfirmationHistory(newClosingTicket, ticketConcernExist, command, cancellationToken);

                await TransactionNotification(newClosingTicket, ticketConcernExist, userDetails, command, cancellationToken);

                ticketConcernExist.IsClosedApprove = false;

            }

            if (!Directory.Exists(TicketingConString.AttachmentPath))
            {
                Directory.CreateDirectory(TicketingConString.AttachmentPath);
            }

            if (command.AddClosingAttachments.Count(x => x.Attachment != null) > 0)
            {
              var attachment =  await AttachmentHandler(closingTicketExist, ticketConcernExist, command, cancellationToken);
                if(attachment is not null)
                    return attachment;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }


        private async Task<ClosingTicket> AddClosingTicket(Approver approver, TicketConcern ticketConcern, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var addNewClosingConcern = new ClosingTicket
            {
                TicketConcernId = ticketConcern.Id,
                Resolution = command.Resolution,
                CategoryId = command.CategoryId,
                SubCategoryId = command.SubCategoryId,
                IsClosing = false,
                TicketApprover = approver.UserId,
                AddedBy = command.Added_By,
                Notes = command.Notes,
            };

            await _context.ClosingTickets.AddAsync(addNewClosingConcern);

            await _context.SaveChangesAsync(cancellationToken);
            return addNewClosingConcern;

        }

        private async Task<ApproverTicketing> AddApproverTicketing(TicketConcern ticketConcern, ClosingTicket closingTicket, Approver approver, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var addNewApprover = new ApproverTicketing
            {
                TicketConcernId = ticketConcern.Id,
                ClosingTicketId = closingTicket.Id,
                UserId = approver.UserId,
                ApproverLevel = approver.ApproverLevel,
                AddedBy = command.Added_By,
                CreatedAt = DateTime.Now,
                Status = TicketingConString.CloseTicket,
            };

            await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);

            return addNewApprover;

        }

        private async Task<Result?> ValidationHandler(List<Approver> approver, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            if (!approver.Any())
                return Result.Failure(ClosingTicketError.NoApproverHasSetup());

            var categoryExist = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == command.CategoryId,cancellationToken);
            if (categoryExist is null)
                return Result.Failure(TicketRequestError.CategoryNotExist());

            var subCategoryExist = await _context.SubCategories
                .FirstOrDefaultAsync(x => x.Id == command.SubCategoryId, cancellationToken);
            if (subCategoryExist is null)
                return Result.Failure(TicketRequestError.SubCategoryNotExist());

            return null;

        }

        private async Task<TicketHistory> AddClosingHistory(TicketConcern ticketConcern, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var addTicketHistory = new TicketHistory
            {
                TicketConcernId = ticketConcern.Id,
                TransactedBy = command.Added_By,
                TransactionDate = DateTime.Now,
                Request = TicketingConString.ForClosing,
                Status = TicketingConString.CloseRequest
            };

            await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

            return addTicketHistory;
        }

        private async Task AddApproverHistory(TicketConcern ticketConcern, Approver approver, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var approverLevel = approver.ApproverLevel == 1 ? $"{approver.ApproverLevel}st"
                : approver.ApproverLevel == 2 ? $"{approver.ApproverLevel}nd"
                : approver.ApproverLevel == 3 ? $"{approver.ApproverLevel}rd"
                : $"{approver.ApproverLevel}th";

            var addApproverHistory = new TicketHistory
            {
                TicketConcernId = ticketConcern.Id,
                TransactedBy = approver.UserId,
                TransactionDate = DateTime.Now,
                Request = TicketingConString.Approval,
                Status = $"{TicketingConString.CloseForApproval} {approverLevel} Approver",
                Approver_Level = approver.ApproverLevel,
            };

            await _context.TicketHistories.AddAsync(addApproverHistory, cancellationToken);

        }

        private async Task<TicketHistory> AddConfirmationHistory(ClosingTicket closingTicket, TicketConcern ticketConcern, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {

            var businessUnitList = await _context.BusinessUnits
                .FirstOrDefaultAsync(x => x.Id == closingTicket.TicketConcern.User.BusinessUnitId);

            var receiverList = await _context.Receivers
                .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

            var addForConfirmationHistory = new TicketHistory
            {
                TicketConcernId = closingTicket.TicketConcernId,
                TransactedBy = closingTicket.TicketConcern.RequestorBy,
                TransactionDate = DateTime.Now,
                Request = TicketingConString.NotConfirm,
                Status = $"{TicketingConString.CloseForConfirmation} {ticketConcern.RequestorByUser.Fullname}",
            };

            await _context.TicketHistories.AddAsync(addForConfirmationHistory, cancellationToken);

            return addForConfirmationHistory;
        }

        private async Task<TicketTransactionNotification> TransactionNotification(ClosingTicket closingTicket, TicketConcern ticketConcern, User user, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var addNewTicketTransactionNotification = new TicketTransactionNotification
            {

                Message = $"Ticket number {ticketConcern.Id} is pending for closing approval",
                AddedBy = user.Id,
                Created_At = DateTime.Now,
                ReceiveBy = closingTicket.TicketApprover.Value,
                Modules = PathConString.Approval,
                Modules_Parameter = PathConString.ForClosingTicket,
                PathId = ticketConcern.Id

            };

            await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            return addNewTicketTransactionNotification;

        }

        private async Task<Result?> AttachmentHandler(ClosingTicket closingTicket, TicketConcern ticketConcern, AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {

            foreach (var attachments in command.AddClosingAttachments.Where(a => a.Attachment.Length > 0))
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
                        ClosingTicketId = closingTicket.Id,
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

        private async Task<ClosingTicket> UpdateClosingTicket(ClosingTicket closingTicket ,AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            bool IsChanged = false;

            if (closingTicket.Resolution != command.Resolution)
            {
                closingTicket.Resolution = command.Resolution;
                IsChanged = true;
            }

            if (closingTicket.Notes != command.Notes)
            {
                closingTicket.Notes = command.Notes;
                IsChanged = true;
            }
            if (closingTicket.CategoryId != command.CategoryId)
            {
                closingTicket.CategoryId = command.CategoryId;
                IsChanged = true;
            }
            if (closingTicket.SubCategoryId != command.SubCategoryId)
            {
                closingTicket.SubCategoryId = command.SubCategoryId;
                IsChanged = true;
            }

            if (IsChanged)
            {
                closingTicket.ModifiedBy = command.Modified_By;
                closingTicket.UpdatedAt = DateTime.Now;
            }

            return closingTicket;
        }

    }

}
