using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two
{
    public class QuestionaireTransactionId : BaseIdEntity
    {
        public int PmsQuestionaireModuleId { get; set; }
        public virtual PmsQuestionaireModule PmsQuestionaireModule { get; set; }

        public int PmsQuestionId {  get; set; }
        public virtual PmsQuestionaire PmsQuestionaire { get; set; }

        public bool IsDeleted { get; set; } = false;




    }
}
