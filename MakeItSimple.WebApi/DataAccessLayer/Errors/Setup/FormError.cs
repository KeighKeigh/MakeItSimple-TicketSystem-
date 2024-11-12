using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class FormError
    {
        public static Error FormNameExist() =>
        new Error("Form.FormNameExist", "Form name already exist!");
        public static Error FormNotExist() =>
        new Error("Form.FormNotExist", "Form not exist!");
        public static Error QuestionCategoryNotExist() =>
        new Error("Form.QuestionCategoryNotExist", "Question category not exist!");

        public static Error QuestionCategoryAlreadyExist() =>
        new Error("Form.QuestionCategoryAlreadyExist", "Question category already exist!");

    }
}
