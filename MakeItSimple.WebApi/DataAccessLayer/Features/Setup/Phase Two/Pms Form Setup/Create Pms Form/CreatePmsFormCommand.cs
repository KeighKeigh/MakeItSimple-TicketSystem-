using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form
{
    public partial class CreatePmsForm 
    {
        public class CreatePmsFormCommand : IRequest<Result>
        {
            public string Form_Name { get; set; }
            public Guid? Added_By { get; set; }

        }
    }
}
