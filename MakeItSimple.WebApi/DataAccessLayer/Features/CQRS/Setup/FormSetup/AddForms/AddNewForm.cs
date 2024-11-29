using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.AddForms
{
    public partial class AddNewForm
    {

        public class Handler : IRequestHandler<AddNewFormCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewFormCommand command, CancellationToken cancellationToken)
            {

                var formExist = await _context.Forms
                    .FirstOrDefaultAsync(f => f.Id == command.Id);

                if (formExist is not null)
                {
                    var formNameExist = await _context.Forms
                        .FirstOrDefaultAsync(f => f.Form_Name == command.Form_Name
                        && !f.Form_Name.Equals(formExist.Form_Name));

                    if (formNameExist is not null)
                        return Result.Failure(FormError.FormNameExist());
                      
                    formExist.Form_Name = command.Form_Name;
                    formExist.ModifiedBy = command.Modified_By;

                }
                else
                {
                    var formNameExist = await _context.Forms
                        .FirstOrDefaultAsync(f => f.Form_Name == command.Form_Name);

                    if (formNameExist is not null)
                        return Result.Failure(FormError.FormNameExist());
                    
                    var addNewForms = new Form
                    {
                        Form_Name = command.Form_Name,
                        AddedBy = command.Added_By,

                    };

                    await _context.Forms.AddAsync(addNewForms, cancellationToken);

                }


                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
