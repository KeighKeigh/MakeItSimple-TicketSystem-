﻿using System.Collections.ObjectModel;

namespace MakeItSimple.WebApi.Models.Ticketing.TicketRequest
{
    public class RequestGenerator
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<TicketConcern> TicketConcerns { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }

    }
}