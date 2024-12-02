using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form_Status
{
    public class UpdatePmsFormStatus
    {
        public class UpdatePmsFormStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdatePmsFormStatusCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsFormStatusCommand command, CancellationToken cancellationToken)
            {
                var pmsFormIdNotExist = await unitOfWork.PmsForm.PmsFormIdNotExist(command.Id);
                if (pmsFormIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());


                await unitOfWork.PmsForm.UpdateStatus(command.Id, pmsFormIdNotExist.IsActive);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
