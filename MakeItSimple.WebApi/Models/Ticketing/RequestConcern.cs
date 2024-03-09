﻿using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class RequestConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public string ConcernStatus { get; set; }

        public bool IsReject { get; set; } = false;
        public Guid? RejectBy { get; set; }
        public virtual User RejectByUser { get; set; }

        public bool ? IsDone { get; set; }

        public string Concern { get; set; }

        public string Remarks { get; set; }

        public int ? RequestGeneratorId { get; set; }
        public virtual RequestGenerator RequestGenerator { get; set; }
        public ICollection<TicketConcern> TicketConcerns { get; set; }


    }
}
