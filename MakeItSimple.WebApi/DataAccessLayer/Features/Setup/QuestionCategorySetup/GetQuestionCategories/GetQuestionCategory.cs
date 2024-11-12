using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.GetQuestionCategories
{
    public partial class GetQuestionCategory
    {

        public class Handler : IRequestHandler<GetQuestionCategoryQuery, PagedList<GetQuestionCategoryResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetQuestionCategoryResult>> Handle(GetQuestionCategoryQuery request, CancellationToken cancellationToken)
            {
                IQueryable<QuestionCategory> questionCategories = _context.QuestionCategories
                    .AsNoTrackingWithIdentityResolution()
                   
                    .Include(q => q.Form)
                    .AsSplitQuery();

                if (!string.IsNullOrEmpty(request.Search))
                    questionCategories = questionCategories
                        .Where(q => q.QuestionCategoryName.Contains(request.Search));

                if (request.Status is not null)
                    questionCategories = questionCategories
                        .Where(q => q.IsActive == request.Status);

                var results = questionCategories
                    .Select(r => new GetQuestionCategoryResult
                    {
                        Id = r.Id,  
                        FormId = r.FormId,
                        Form_Name = r.Form.Form_Name,
                        Question_Category_Name = r.QuestionCategoryName,
                        Added_By = r.AddedByUser.Fullname,
                        Created_At = r.CreatedAt,
                        Modified_By = r.ModifiedByUser.Fullname,
                        Updated_At = r.UpdatedAt,
                        Is_Active = r.IsActive,
                   
                    });

                return await PagedList<GetQuestionCategoryResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
