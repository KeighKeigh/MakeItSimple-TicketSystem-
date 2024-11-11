using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.UpdateStatus
{
    public partial class UpdateFormStatus
    {
        public class UpdateFormStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
