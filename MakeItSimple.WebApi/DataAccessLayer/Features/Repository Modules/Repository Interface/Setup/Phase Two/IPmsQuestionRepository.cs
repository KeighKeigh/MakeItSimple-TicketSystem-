using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire.CreatePmsQuestion;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two
{
    public interface IPmsQuestionRepository
    {
        Task<bool> PmsQuestionAlreadyExist(string pmsQuestion , string currentQuestion);
        Task<bool> PmsQuestionTypeAlreadyExist(string pmsQuestionType, string currentQuestionType);
        Task<PmsQuestionaire> PmsQuestionNotExist(int id);
        Task<bool> QuestionTypeNotExist(string questionType);
        Task<PmsQuestionaire> CreatePmsQuestion(CreatePmsQuestionCommand pmsQuestion);
        Task CreateQuestionTransaction(CreatePmsQuestionCommand.PmsQuestionModule pmsForm, PmsQuestionaire question);
        Task CreateQuestionType(CreatePmsQuestionCommand.PmsQuestionType pmsQuestionType , PmsQuestionaire question, string questionType);
    }
}
