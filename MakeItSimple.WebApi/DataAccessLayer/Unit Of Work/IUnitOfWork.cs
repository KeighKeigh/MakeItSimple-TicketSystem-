using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;

namespace MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        IPmsFormRepository PmsForm { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        
    }
}
