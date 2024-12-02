using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module
{
    public partial class UpdatePmsQuestionaireModule
    {
        public class UpdatePmsQuestionaireModuleCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public int PmsFormId { get; set; }
            public string Questionaire_Module_Name { get; set; }
            public Guid? Modified_By { get; set; }
        }
    }
}
