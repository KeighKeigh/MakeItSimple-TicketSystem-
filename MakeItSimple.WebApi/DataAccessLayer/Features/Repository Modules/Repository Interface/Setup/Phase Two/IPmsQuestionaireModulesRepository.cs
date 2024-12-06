using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.CreatePmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module.UpdatePmsQuestionaireModule;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two
{
    public interface IPmsQuestionaireModulesRepository 
    {
        Task CreateQuestionaireModule(CreatePmsQuestionaireModuleCommand pmsQModules);
        Task<bool> QuestionaireModuleNameAlreadyExist(string pmsQModuleName);

        IQueryable<PmsQuestionaireModule> SearchPmsForm(string search);

        IQueryable<PmsQuestionaireModule> ArchivedPmsForm(bool? is_Archived);

        IQueryable<PmsQuestionaireModule> OrdersPmsForm(string order_By);

        Task<PmsQuestionaireModule> PmsQuestionaireModuleIdNotExist(int id);

        Task UpdatePmsQuestionaireModule(UpdatePmsQuestionaireModuleCommand pmsQModules);

        Task UpdatePmsQuestionaireModuleStatus(int id , bool status);

    }
}
