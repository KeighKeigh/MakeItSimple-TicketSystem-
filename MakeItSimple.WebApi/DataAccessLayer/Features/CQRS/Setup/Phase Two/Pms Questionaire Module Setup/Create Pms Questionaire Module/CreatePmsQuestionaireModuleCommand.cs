using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup
{
    public partial class CreatePmsQuestionaireModule
    {
        public class CreatePmsQuestionaireModuleCommand : IRequest<Result>
        {
            public int PmsFormId { get; set; }
            public string Questionaire_Module_Name { get; set; }
            public Guid? Added_By { get; set; }

        }
    }
}
