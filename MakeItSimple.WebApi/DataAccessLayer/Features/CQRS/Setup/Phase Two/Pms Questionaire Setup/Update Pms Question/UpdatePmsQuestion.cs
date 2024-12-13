using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Update_Pms_Question
{
    public class UpdatePmsQuestion
    {
        public class UpdatePmsQuestionCommand : IRequest<Result>
        {
         
            public int Id { get; set; }
            public string Question { get; set; }
            public string Question_Type { get; set; }
            public Guid? Modified_By { get; set; }
            public List<PmsQuestionModule> PmsQuestionModules { get; set; }

            public class PmsQuestionModule
            {
                public int? QuestionTransactionId { get; set; }
                public int PmsQuestionModuleId { get; set; }

            }

            public List<PmsQuestionType> PmsQuestionTypes { get; set; }
            public class PmsQuestionType
            {
                public int? Id { get; set; }
                public string Description { get; set; }
            }
        }

    }

}
