using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire.CreatePmsQuestion;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two
{
    public interface IPmsQuestionRepository
    {
        Task<PmsQuestionaire> CreatePmsQuestion(CreatePmsQuestionCommand pmsQuestion);

        Task CreateQuestionTransaction(CreatePmsQuestionCommand.PmsForm pmsForm, int id);
    }
}
