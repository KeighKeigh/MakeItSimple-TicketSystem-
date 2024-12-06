
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

            var nextId = await context.Database
                .ExecuteSqlRawAsync("SELECT IDENT_CURRENT('pms_questionaire_modules') + 1");

            var create = new PmsQuestionaire
            {
                Id = nextId,
                Question = pmsQuestion.Question,
                AddedBy = pmsQuestion.Added_By,
            };

            await context.PmsQuestionaires.AddAsync(create);

            return create;
        }

        public async Task CreateQuestionTransaction(CreatePmsQuestionCommand.PmsForm pmsForm, int id)
        {
            var create = new QuestionTransactionId
            {
                PmsQuestionaireModuleId = pmsForm.PmsFormId,
                PmsQuestionId = id,
            };

        }
    }
}
