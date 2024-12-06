using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two
{
    public class PmsQuestionaire : BaseIdEntity
    {
        [Required]
        public string Question {  get; set; }
        public  string QuestionType { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public ICollection<QuestionTransactionId> QuestionTransactionIds { get; set; }

    }
}

