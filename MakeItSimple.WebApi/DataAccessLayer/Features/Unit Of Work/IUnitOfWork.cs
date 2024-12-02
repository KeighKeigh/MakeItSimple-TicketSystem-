using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;

namespace MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        IPmsFormRepository PmsForm { get; }
        IPmsQuestionaireModulesRepository PmsQuestionaireModules { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        
    }
}
