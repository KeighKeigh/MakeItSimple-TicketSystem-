using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.FormCheckBoxSetup;
using MakeItSimple.WebApi.Models.Setup.FormDropdownSetup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;

namespace MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup
{
    public class FormQuestion : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public string Question {  get; set; }
        public int QuestionCategoryId { get; set; }
        public virtual QuestionCategory QuestionCategory { get; set; }
        public string QuestionType { get; set; }

        public ICollection<FormCheckBox> FormCheckBoxes { get; set; }
        public ICollection<FormDropdown> FormDropdowns { get; set; }

        
    }
}
