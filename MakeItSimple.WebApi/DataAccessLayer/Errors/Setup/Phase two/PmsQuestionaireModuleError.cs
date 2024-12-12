using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two
{
    public class PmsQuestionaireModuleError
    {
        public static Error PmsQuestionaireModuleAlreadyExist() =>
        new Error("PmsForm.PmsQuestionaireModuleAlreadyExist", "Pms questionaire module already exist!");
        public static Error PmsQuestionaireModuleIdNotExist() =>
        new Error("PmsForm.PmsQuestionaireModuleIdNotExist", "Pms questionaire module not exist!");

    }
}
