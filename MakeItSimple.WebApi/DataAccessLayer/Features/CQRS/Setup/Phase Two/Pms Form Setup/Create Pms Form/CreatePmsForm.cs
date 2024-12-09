using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form
{
    public partial class CreatePmsForm 
    {

        public class Handler : IRequestHandler<CreatePmsFormCommand, Result>
        {
            private readonly IUnitOfWork _unitOfWork;
            
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CreatePmsFormCommand command, CancellationToken cancellationToken)
            {

               var formNameAlreadyExist =  await _unitOfWork.PmsForm.FormNameAlreadyExist(command.Form_Name,null);
                if (formNameAlreadyExist)
                    return Result.Failure(PmsFormError.PmsFormAlreadyExist());

                await _unitOfWork.PmsForm.CreatePmsForm(command);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
