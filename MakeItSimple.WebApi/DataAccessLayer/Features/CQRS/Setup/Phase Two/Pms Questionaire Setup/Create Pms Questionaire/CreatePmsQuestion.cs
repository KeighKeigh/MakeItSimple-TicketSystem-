using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire
{
    public partial class CreatePmsQuestion
    {

        public class Handler : IRequestHandler<CreatePmsQuestionCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CreatePmsQuestionCommand command, CancellationToken cancellationToken)
            {
                var newPmsQuestion = unitOfWork.PmsQuestion.CreatePmsQuestion(command);

                foreach (var pmsForm in command.PmsForms)
                {
                    await unitOfWork.PmsQuestion.CreateQuestionTransaction(pmsForm, newPmsQuestion.Id);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
