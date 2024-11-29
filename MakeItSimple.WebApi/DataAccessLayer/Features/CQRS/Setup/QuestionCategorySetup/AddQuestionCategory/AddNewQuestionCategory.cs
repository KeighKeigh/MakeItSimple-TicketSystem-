using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.AddQuestionCategory
{
    public partial class AddNewQuestionCategory
    {

        public class Handler : IRequestHandler<AddNewQuestionCategoryCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewQuestionCategoryCommand command, CancellationToken cancellationToken)
            {

                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateQuestionCategory(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
               
            }

            private async Task<Result?> Validator(AddNewQuestionCategoryCommand command, CancellationToken cancellationToken)
            {
                var formExist = await _context.Forms
                    .FirstOrDefaultAsync(f => f.Id == command.FormId, cancellationToken);

                if (formExist is null)
                    return Result.Failure(FormError.FormNotExist());

                var questionCategoriesAlreadyExist = await _context.QuestionCategories
                    .FirstOrDefaultAsync(q => q.FormId == command.FormId && q.QuestionCategoryName == command.Question_Category_Name, cancellationToken);

                if (questionCategoriesAlreadyExist is not null)
                    return Result.Failure(FormError.QuestionCategoryAlreadyExist());

                return null;

            }

            private async Task CreateQuestionCategory(AddNewQuestionCategoryCommand command , CancellationToken cancellationToken)
            {
                var addFormCategory = new QuestionCategory
                {
                    FormId = command.FormId,
                    QuestionCategoryName = command.Question_Category_Name,
                    AddedBy = command.Added_By,

                };

                await _context.QuestionCategories.AddAsync(addFormCategory);
            }


        }
    }
}
