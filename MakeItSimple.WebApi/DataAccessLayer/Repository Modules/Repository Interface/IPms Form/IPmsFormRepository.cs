using MakeItSimple.WebApi.DataAccessLayer.Dto.Pms_Form_Dto;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form
{
    public interface IPmsFormRepository
    {
        void CreatePmsForm(CreatePmsFormCommand pmsForm);
    }
}
