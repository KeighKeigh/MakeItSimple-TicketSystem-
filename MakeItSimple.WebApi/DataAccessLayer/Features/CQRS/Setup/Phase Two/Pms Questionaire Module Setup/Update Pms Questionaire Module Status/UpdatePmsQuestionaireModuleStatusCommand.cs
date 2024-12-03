using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module
{
    public partial class UpdatePmsQuestionaireModuleStatus
    {
        public class UpdatePmsQuestionaireModuleStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
