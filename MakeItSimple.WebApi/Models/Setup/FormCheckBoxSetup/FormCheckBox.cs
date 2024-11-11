using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;

namespace MakeItSimple.WebApi.Models.Setup.FormCheckBoxSetup
{
    public class FormCheckBox : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public string CheckBoxDescription { get; set; }

        public int FormQuestionId { get; set; }
        public virtual FormQuestion FormQuestion { get; set; }
    }
}
