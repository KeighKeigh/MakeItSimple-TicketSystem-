using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategories
{
    public partial class UpdateQuestionCategory
    {

        public class Handler : IRequestHandler<UpdateQuestionCategoryCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateQuestionCategoryCommand command, CancellationToken cancellationToken)
            {
                var questionCategoryExist = await _context.QuestionCategories
                    .FirstOrDefaultAsync(q => q.Id == command.Id, cancellationToken);

                var validator = await Validator(questionCategoryExist,command,cancellationToken);  
                if (validator is not null) 
                    return validator;

                await UpdateQuestionCategory(questionCategoryExist,command, cancellationToken); 

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();    
            }

            private async Task<Result?> Validator(QuestionCategory questionCategory, UpdateQuestionCategoryCommand command,CancellationToken cancellationToken )
            {
                if (questionCategory is null)
                    return Result.Failure(FormError.QuestionCategoryNotExist());

                var formExist = await _context.Forms
                    .FirstOrDefaultAsync(f => f.Id == command.FormId, cancellationToken);

                if (formExist is null)
                    return Result.Failure(FormError.FormNotExist());

                var questionCategoriesAlreadyExist = await _context.QuestionCategories
                    .AnyAsync(q => q.FormId == command.FormId && 
                    string.Equals(q.QuestionCategoryName, command.Question_Category_Name)
                    && !string.Equals(questionCategory.QuestionCategoryName, command.Question_Category_Name));

                if (questionCategoriesAlreadyExist)
                    return Result.Failure(FormError.QuestionCategoryAlreadyExist());

                return null;
            }

            private async Task UpdateQuestionCategory(QuestionCategory questionCategory, UpdateQuestionCategoryCommand command, CancellationToken cancellationToken)
            {
                questionCategory.FormId = command.FormId;
                questionCategory.QuestionCategoryName = command.Question_Category_Name;
                questionCategory.ModifiedBy = command.Modified_By;
                questionCategory.UpdatedAt = DateTime.Now;
            }
        }
    }
}
