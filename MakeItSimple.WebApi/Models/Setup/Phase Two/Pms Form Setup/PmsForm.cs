using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup
{
    public class PmsForm : BaseIdEntity 
    {
        public string Form_Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
    }
}
