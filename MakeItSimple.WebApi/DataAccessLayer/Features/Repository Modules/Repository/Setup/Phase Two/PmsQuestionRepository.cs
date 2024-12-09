
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire;
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
                AddedBy = pmsQuestion.Added_By,
            };

            await context.PmsQuestionaires.AddAsync(create);

            return create;
        }

        public async Task CreateQuestionTransaction(CreatePmsQuestionCommand.PmsQuestionModule pmsForm, int id)
        {
            var create = new QuestionTransactionId
            {
                PmsQuestionaireModuleId = pmsForm.PmsQuestionModuleId,
                PmsQuestionId = id,
            };

            await context.QuestionTransactionIds.AddAsync(create);
        }

        public async Task CreateQuestionType(CreatePmsQuestionCommand.PmsQuestionType pmsQuestionType, int id, string questionType)
        {
            var create = new PmsQuestionType
            {
                Description = pmsQuestionType.Description,
                PmsQuestionaireId = id,
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

    }
}
