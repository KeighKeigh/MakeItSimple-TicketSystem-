using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategoriesStatus
{
    public partial class UpdateQuestionCategoryStatus
    {

        public class Handler : IRequestHandler<UpdateQuestionCategoryStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateQuestionCategoryStatusCommand request, CancellationToken cancellationToken)
            {
                var questionCategory = await _context.QuestionCategories
                    .FirstOrDefaultAsync(q => q.Id == request.Id,cancellationToken);

                if (questionCategory == null)
                    return Result.Failure(FormError.QuestionCategoryNotExist());

                questionCategory.IsActive = !questionCategory.IsActive;

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
