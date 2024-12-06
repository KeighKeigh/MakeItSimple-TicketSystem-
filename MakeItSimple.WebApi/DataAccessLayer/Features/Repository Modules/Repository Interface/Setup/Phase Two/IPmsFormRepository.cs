using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form.UpdatePmsForm;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form
{
    public interface IPmsFormRepository
    {
        Task CreatePmsForm(CreatePmsFormCommand pmsForm);

        Task<bool> FormNameAlreadyExist(string form);

        IQueryable<PmsForm> SearchPmsForm(string search);

        IQueryable<PmsForm> ArchivedPmsForm(bool? is_Archived);

        IQueryable<PmsForm> OrdersPmsForm(string order_By);

        Task<PmsForm> PmsFormIdNotExist(int id);

        Task UpdatePmsForm(UpdatePmsFormCommand pmsForm);

        Task UpdateStatus(int id,bool status);


    }
}
