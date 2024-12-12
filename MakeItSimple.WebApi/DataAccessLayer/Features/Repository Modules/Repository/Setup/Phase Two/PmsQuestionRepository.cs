using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Enumerator;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire.CreatePmsQuestion;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two
{
    public class PmsQuestionRepository : IPmsQuestionRepository
    {
        private readonly MisDbContext context;

        public PmsQuestionRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<PmsQuestionaire> CreatePmsQuestion(CreatePmsQuestionCommand pmsQuestion)
        {

            var create = new PmsQuestionaire
            {
                Question = pmsQuestion.Question,
                QuestionType = pmsQuestion.Question_Type,
                AddedBy = pmsQuestion.Added_By,
            };

            await context.PmsQuestionaires.AddAsync(create);

            return create;
        }

        public async Task CreateQuestionTransaction(CreatePmsQuestionCommand.PmsQuestionModule pmsForm, PmsQuestionaire question)
        {
            var create = new QuestionTransactionId
            {
                PmsQuestionaireModuleId = pmsForm.PmsQuestionModuleId,
                PmsQuestionaire = question,
            };

            await context.QuestionTransactionIds.AddAsync(create);
        }

        public async Task CreateQuestionType(CreatePmsQuestionCommand.PmsQuestionType pmsQuestionType,PmsQuestionaire question, string questionType)
        {
            var create = new PmsQuestionType
            {
                Description = questionType.Contains(PmsConsString.TextType) ? "" : pmsQuestionType.Description,
                PmsQuestionaire = question,
                QuestionType = questionType,

            };

            await context.PmsQuestionTypes.AddAsync(create);
        }

        public async Task<bool> PmsQuestionAlreadyExist(string pmsQuestion, string currentQuestion)
        {
            if(string.IsNullOrEmpty(currentQuestion))
                return await context.PmsQuestionaires
                    .AnyAsync(x => x.Question == pmsQuestion);

            return await context.PmsQuestionaires
                .Where(x => x.Question == pmsQuestion
                && !pmsQuestion.Equals(currentQuestion)) 
                .AnyAsync();
        }

        public async Task<bool> PmsQuestionTypeAlreadyExist(string pmsQuestionType, string currentQuestionType)
        {
            if (string.IsNullOrEmpty(currentQuestionType))
                return await context.PmsQuestionTypes
                    .AnyAsync(x => x.Description == pmsQuestionType);

            return await context.PmsQuestionTypes
                .Where(x => x.Description == pmsQuestionType
                && !pmsQuestionType.Equals(currentQuestionType))
                .AnyAsync();
        }

        public async Task<PmsQuestionaire> PmsQuestionNotExist(int id)
        {
            return await context.PmsQuestionaires.FindAsync(id);
        }

        public async Task<bool> QuestionTypeNotExist(string questionType)
        {

            bool doesNotExist = Enum.TryParse(questionType,out QuestionTypeEnumerator result) ? true : false;
            return await Task.FromResult(doesNotExist);

        }
    }
}
