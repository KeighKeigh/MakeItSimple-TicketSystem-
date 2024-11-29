using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class PmsFormError
    {
        public static Error PmsFormAlreadyExist() =>
        new Error("PmsForm.PmsFormAlreadyExist", "Pms form already exist!");
    }
}
