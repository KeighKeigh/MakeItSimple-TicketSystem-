using MakeItSimple.WebApi.Common;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup
{
    public class PmsForm : BaseIdEntity 
    {
        [Required]
        public string Form_Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public ICollection<PmsQuestionaireModule> PmsQuestionaireModules { get; set; }
       
    }
}
