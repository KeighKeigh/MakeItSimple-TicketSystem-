using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketSubCategory
    {

        public int Id { get; set; }
        public int RequestConcernId { get; set; }
        public virtual RequestConcern RequestConcern { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public bool? IsRemoved { get; set; }
    }
}
