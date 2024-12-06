using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire
{
    public partial class CreatePmsQuestion
    {
        public class CreatePmsQuestionCommand : IRequest<Result>
        {
            public string Question  { get; set; }
            public string Question_Type { get; set; }
            public Guid? Added_By { get; set; }
            public List<PmsForm> PmsForms { get; set; }

            public class PmsForm
            {
                public int PmsFormId { get; set; }

            }

        }
    }
}
