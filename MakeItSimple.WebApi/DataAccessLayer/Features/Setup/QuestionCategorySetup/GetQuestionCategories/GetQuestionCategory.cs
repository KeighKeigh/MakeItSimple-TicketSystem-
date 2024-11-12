using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.GetQuestionCategories
{
    public class GetQuestionCategory
    {
        public record GetQuestionCategoryResult
        {
            public int Id { get; set; }
            public int FormId { get; set; }
            public string Form_Name { get; set; }
            public string Question_Category_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string Is_Active { get; set; }

        }

        public class GetQuestionCategoryQuery : UserParams, IRequest<PagedList<GetQuestionCategoryResult>>
        {
            public string Search {  get; set; }
            public bool? Status { get; set; }

        }

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
                        Added_By = r.AddedByUser.Fullname,


                    });

                return await PagedList<GetQuestionCategoryResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
