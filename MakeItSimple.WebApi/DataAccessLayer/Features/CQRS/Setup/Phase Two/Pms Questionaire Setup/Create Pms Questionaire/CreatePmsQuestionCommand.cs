using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire
{
    public partial class CreatePmsQuestion
    {
        public class CreatePmsQuestionCommand : IRequest<Result>
        {
            public string Question  { get; set; }
            public string Question_Type { get; set; }
            public Guid? Added_By { get; set; }
            public List<PmsQuestionModule> PmsQuestionModules { get; set; }

            public class PmsQuestionModule
            {
                public int PmsQuestionModuleId { get; set; }

            }

            public List<PmsQuestionType> PmsQuestionTypes { get; set; }
            public class PmsQuestionType
            {
                public string Description { get; set; }
            }

        }
    }
}
