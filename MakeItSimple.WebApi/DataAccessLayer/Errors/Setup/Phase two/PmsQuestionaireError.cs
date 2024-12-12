using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two
{
    public class PmsQuestionaireError
    {
        public static Error PmsQuestionAlreadyExist() =>
       new Error("PmsQuestionaire.PmsQuestionAlreadyExist", "Pms question already exist!");

        public static Error PmsQuestionIdNotExist() =>
        new Error("PmsQuestionaire.PmsQuestionAlreadyExist", "Pms question not exist!");

        public static Error PmsQuestionTypeAlreadyExist() =>
        new Error("PmsQuestionaire.PmsQuestionAlreadyExist", "Pms question type already exist!");

        public static Error PmsQuestionTypeRequired() =>
        new Error("PmsQuestionaire.PmsQuestionTypeRequired", "Pms question type required!");

        public static Error PmsQuestionModuleDuplicated() =>
        new Error("PmsQuestionaire.PmsQuestionModuleDuplicated", "Pms question module was duplicate!");

        public static Error PmsQuestionTypeDuplicated() =>
        new Error("PmsQuestionaire.PmsQuestionTypeDuplicated", "Pms question type was duplicate!");
        public static Error PmsQuestionTypeNotExist() =>
        new Error("PmsQuestionaire.PmsQuestionTypeNotExist", "Pms question type not exist!");

    }
}
