using MakeItSimple.WebApi.Common;
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

                await _unitOfWork.PmsForm.CreatePmsForm(command);

                return Result.Success();
            }
        }
    }
}
