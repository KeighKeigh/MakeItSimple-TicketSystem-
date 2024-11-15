using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ViewMultipleSubCategories
{
    public class ViewMultipleSubCategory
    {

        public record ViewMultipleCategoryResult
        {
            public int CategoryId { get; set; }
            public string Category_Description { get; set; }

            public List<ViewMultipleSubCategoryResult> ViewMultipleSubCategoryResults { get; set; }

            public record ViewMultipleSubCategoryResult
            {
                public int SubCategoryId { get; set; }
                public string Sub_Category_Description { get; set; }

            }


        }

        

        public class ViewMultipleSubCategoryQuery : IRequest<Result>
        {
            public int[] CategoryId { get; set; }

        }

        public class Handler : IRequestHandler<ViewMultipleSubCategoryQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ViewMultipleSubCategoryQuery request, CancellationToken cancellationToken)
            {
                var subCategoryList = new Dictionary<int, List<SubCategory>>();

                foreach(var category in request.CategoryId)
                {
                    var categoriesList = await _context.SubCategories
                        .Include(s => s.Category)
                        .Where(s => s.CategoryId == category)
                        .ToListAsync();

                    subCategoryList.Add(category, categoriesList);

                }

                var result = subCategoryList
                     .Select(r => new ViewMultipleCategoryResult
                     {
                         CategoryId = r.Key,
                         Category_Description = r.Value.First().Category.CategoryDescription,
                         ViewMultipleSubCategoryResults = r.Value.Select(v => new ViewMultipleCategoryResult.ViewMultipleSubCategoryResult
                         {
                             SubCategoryId = v.Id,
                             Sub_Category_Description = v.SubCategoryDescription,

                         }).ToList(),

                     }).ToList();

                return Result.Success(result);
            }
        }
    }
}
