using DocumentFormat.OpenXml.Drawing.Charts;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketCategory
    {
        public int Id { get; set; }
        public int RequestConcernId { get; set; }
        public virtual RequestConcern RequestConcern { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool ? IsRemoved { get; set; }

    }
}
