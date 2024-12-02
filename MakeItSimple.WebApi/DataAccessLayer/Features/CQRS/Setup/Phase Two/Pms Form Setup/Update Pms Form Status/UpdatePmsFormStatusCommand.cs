using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form_Status
{
    public partial class UpdatePmsFormStatus
    {
        public class UpdatePmsFormStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
