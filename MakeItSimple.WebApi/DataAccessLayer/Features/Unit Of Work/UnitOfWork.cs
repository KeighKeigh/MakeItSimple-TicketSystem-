using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository.Pms_Form;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;

namespace MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MisDbContext context;

        public UnitOfWork(MisDbContext context)
        {
            this.context = context;
            PmsForm = new PmsFormRepository(context);
            PmsQuestionaireModules = new PmsQuestionaireModuleRepository(context);
            PmsQuestion = new PmsQuestionRepository(context);
        }

        public IPmsFormRepository PmsForm { get; private set; }
        public IPmsQuestionaireModulesRepository PmsQuestionaireModules { get; private set; }

        public IPmsQuestionRepository PmsQuestion { get; }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}