using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form
{
    public partial class UpdatePmsForm
    {

        public class Handler : IRequestHandler<UpdatePmsFormCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsFormCommand command, CancellationToken cancellationToken)
            {

               var pmsIdNotExist = await unitOfWork.PmsForm.PmsFormIdNotExist(command.Id);
                if (pmsIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                var formNameAlreadyExist = await unitOfWork.PmsForm.FormNameAlreadyExist(command.Form_Name);
                if (formNameAlreadyExist)
                    return Result.Failure(PmsFormError.PmsFormAlreadyExist());

               await unitOfWork.PmsForm.UpdatePmsForm(command);
       
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
