namespace MakeItSimple.WebApi.Models.Setup.Phase_Two
{
    public class PmsQuestionType :BaseIdEntity
    {
        public int PmsQuestionaireId { get; set; }
        public virtual PmsQuestionaire PmsQuestionaire { get; set; }
        public string QuestionType { get; set; }
        public string Description { get; set; }
        public bool Is_Deleted { get; set; } = false;

    }
}
