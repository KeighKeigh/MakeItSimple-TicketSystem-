using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form
{
    public interface IPmsFormRepository
    {
        Task CreatePmsForm(CreatePmsFormCommand pmsForm);

        Task<bool> FormNameAlreadyExist(string Form);

        Task<IQueryable<PmsForm>> GetPmsForm(string Search);
    }
}
