using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.UpdateStatus
{
    public partial class UpdateFormStatus
    {
        public class Handler : IRequestHandler<UpdateFormStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateFormStatusCommand command, CancellationToken cancellationToken)
            {

                var formExist = await _context.Forms
                    .FirstOrDefaultAsync(f => f.Id == command.Id);

                if (formExist is null)
                    return Result.Failure(FormError.FormNotExist());
                

                formExist.IsActive = !formExist.IsActive;

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
