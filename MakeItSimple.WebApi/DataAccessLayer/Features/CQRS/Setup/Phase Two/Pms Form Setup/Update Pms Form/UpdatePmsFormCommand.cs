using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form
{
    public partial class UpdatePmsForm
    {
        public class UpdatePmsFormCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Form_Name { get; set; }
            public Guid Modified_By { get; set; }

        }
    }
}
