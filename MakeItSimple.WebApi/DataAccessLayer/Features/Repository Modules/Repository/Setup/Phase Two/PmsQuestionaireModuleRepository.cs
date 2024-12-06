using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module.UpdatePmsQuestionaireModule;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Phase_Two
{
    public class PmsQuestionaireModuleRepository : IPmsQuestionaireModulesRepository
    {
        private readonly MisDbContext context;

        public PmsQuestionaireModuleRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task CreateQuestionaireModule(CreatePmsQuestionaireModule.CreatePmsQuestionaireModuleCommand pmsQModules)
        {
            var create = new PmsQuestionaireModule
            {
                QuestionaireModuleName = pmsQModules.Questionaire_Module_Name,
                PmsFormId = pmsQModules.PmsFormId,
                AddedBy = pmsQModules.Added_By,

            };

            await context.PmsQuestionaireModules.AddAsync(create);
        }

        public async Task<bool> QuestionaireModuleNameAlreadyExist(string pmsQModuleName)
        {
            return await context.PmsQuestionaireModules.AnyAsync(x => x.QuestionaireModuleName.Equals(pmsQModuleName));
        }

        public IQueryable<PmsQuestionaireModule> SearchPmsForm(string search)
        {
            return context.PmsQuestionaireModules.Where(x => x.QuestionaireModuleName.ToLower().Contains(search));
        }
        public IQueryable<PmsQuestionaireModule> ArchivedPmsForm(bool? is_Archived)
        {
            return context.PmsQuestionaireModules.Where(q => q.IsActive == is_Archived);
        }

        public IQueryable<PmsQuestionaireModule> OrdersPmsForm(string order_By)
        {
            var query = context.PmsQuestionaireModules.AsQueryable();

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.Id);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.Id);
                    break;

                default:
                    query = query.OrderBy(x => x.QuestionaireModuleName);
                    break;
            }

            return query;
        }

        public async Task<PmsQuestionaireModule> PmsQuestionaireModuleIdNotExist(int id)
        {
            return await context.PmsQuestionaireModules.FindAsync(id);
        }

        public Task UpdatePmsQuestionaireModule(UpdatePmsQuestionaireModuleCommand pmsQModules)
        {

            context.ChangeTracker.Clear();

            var pmsUpdate = new PmsQuestionaireModule
            {
                Id = pmsQModules.Id,
                QuestionaireModuleName = pmsQModules.Questionaire_Module_Name,
                PmsFormId = pmsQModules.PmsFormId,
                ModifiedBy = pmsQModules.Modified_By,
                UpdatedAt = DateTime.Now,

            };
        
            context.PmsQuestionaireModules.Attach(pmsUpdate);
            context.Entry(pmsUpdate).Property(x => x.QuestionaireModuleName).IsModified = true;
            context.Entry(pmsUpdate).Property(x => x.PmsFormId).IsModified = true;
            context.Entry(pmsUpdate).Property(x => x.ModifiedBy).IsModified = true;
            context.Entry(pmsUpdate).Property(x => x.UpdatedAt).IsModified = true;

            return Task.CompletedTask;
        }

        public Task UpdatePmsQuestionaireModuleStatus(int id, bool status)
        {
            context.ChangeTracker.Clear();

            var pmsUpdate = new PmsQuestionaireModule
            {
                Id = id,
                IsActive = !status,

            };

            context.PmsQuestionaireModules.Attach(pmsUpdate);
            context.Entry(pmsUpdate).Property(x => x.IsActive).IsModified = true;

            return Task.CompletedTask;

        }
    }
}
