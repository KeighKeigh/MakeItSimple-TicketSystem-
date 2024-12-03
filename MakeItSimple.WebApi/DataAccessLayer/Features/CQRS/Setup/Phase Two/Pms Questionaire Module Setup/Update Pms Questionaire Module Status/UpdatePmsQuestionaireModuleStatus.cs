using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module
{
    public partial class UpdatePmsQuestionaireModuleStatus
    {

        public class Handler : IRequestHandler<UpdatePmsQuestionaireModuleStatusCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsQuestionaireModuleStatusCommand command, CancellationToken cancellationToken)
            {
                var pmsQuestionaireModuleNotExist = await unitOfWork.PmsQuestionaireModules
                    .PmsQuestionaireModuleIdNotExist(command.Id);
                if (pmsQuestionaireModuleNotExist is null)
                    return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                await unitOfWork.PmsQuestionaireModules
                    .UpdatePmsQuestionaireModuleStatus(command.Id,pmsQuestionaireModuleNotExist.IsActive);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
                
            }
        }
    }
}
