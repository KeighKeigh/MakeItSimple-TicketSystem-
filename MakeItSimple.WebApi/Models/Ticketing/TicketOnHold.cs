namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketOnHold
    {

        public int Id { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Reason { get; set; }
        public bool ? IsHold { get; set; }
        public DateTime? ResumeAt { get; set; }
        public int? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public string OnHoldRemarks { get; set; }

        public bool IsRejectOnHold { get; set; } 
        public DateTime? RejectOnHoldAt { get; set; }
        public Guid? RejectOnHoldBy { get; set; }
        public virtual User RejectOnHoldByUser { get; set; }
        public string RejectRemarks { get; set; }
        public Guid? TicketApprover { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<ApproverTicketing> ApproverTickets { get; set; }

    }
}
