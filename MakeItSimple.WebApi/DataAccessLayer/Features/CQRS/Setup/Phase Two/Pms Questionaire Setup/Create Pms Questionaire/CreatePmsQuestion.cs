using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
                var questionAlreadyExist = await unitOfWork.PmsQuestion
                    .PmsQuestionAlreadyExist(command.Question,null);

                if (questionAlreadyExist)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionAlreadyExist());

                var newPmsQuestion = await unitOfWork.PmsQuestion.CreatePmsQuestion(command);

                foreach (var pmsQuestionModule in command.PmsQuestionModules)
                {

                    if (command.PmsQuestionModules.Count(x => x.PmsQuestionModuleId == pmsQuestionModule.PmsQuestionModuleId) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionModuleDuplicated());

                    var pmsQuestionModuleNotExist = await unitOfWork.PmsQuestionaireModules
                        .PmsQuestionaireModuleIdNotExist(pmsQuestionModule.PmsQuestionModuleId);
                    if(pmsQuestionModuleNotExist is null)
                        return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                    await unitOfWork.PmsQuestion.CreateQuestionTransaction(pmsQuestionModule, newPmsQuestion.Id);
                }

                foreach (var questionType in command.PmsQuestionTypes)
                {
                    if (command.PmsQuestionTypes.Count(x => x.Description == questionType.Description) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionTypeDuplicated());

                    var pmsQuestionTypeAlreadyExist = await unitOfWork.PmsQuestion
                        .PmsQuestionTypeAlreadyExist(questionType.Description,null);
                    if(pmsQuestionTypeAlreadyExist)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionTypeAlreadyExist());

                    await unitOfWork.PmsQuestion
                        .CreateQuestionType(questionType,newPmsQuestion.Id, command.Question_Type);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
