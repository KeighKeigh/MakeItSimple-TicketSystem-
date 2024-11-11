using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.FormSetup;

namespace MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup
{
    public class QuestionCategory : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public int FormId { get; set; }
        public virtual Form Form { get; set; }
        public string QuestionCategoryName {  get; set; }
    }
}
