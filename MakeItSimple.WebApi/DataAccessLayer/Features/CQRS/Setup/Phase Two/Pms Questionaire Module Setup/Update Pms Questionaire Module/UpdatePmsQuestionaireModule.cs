using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module
{
    public partial class UpdatePmsQuestionaireModule
    {
        public class Handler : IRequestHandler<UpdatePmsQuestionaireModuleCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsQuestionaireModuleCommand command, CancellationToken cancellationToken)
            {
                
                var pmsQuestionaireModuleIdNotExist = await unitOfWork.PmsQuestionaireModules
                    .PmsQuestionaireModuleIdNotExist(command.Id);
                if (pmsQuestionaireModuleIdNotExist is null)
                    return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                var pmsFormIdNotExist = await unitOfWork.PmsForm.PmsFormIdNotExist(command.PmsFormId);
                if (pmsFormIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                var questionaireModuleNameAlreadyExist = await unitOfWork.PmsQuestionaireModules
                    .QuestionaireModuleNameAlreadyExist(command.Questionaire_Module_Name,pmsQuestionaireModuleIdNotExist.QuestionaireModuleName);
                if (questionaireModuleNameAlreadyExist)
                    return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleAlreadyExist());

                await unitOfWork.PmsQuestionaireModules.UpdatePmsQuestionaireModule(command);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
