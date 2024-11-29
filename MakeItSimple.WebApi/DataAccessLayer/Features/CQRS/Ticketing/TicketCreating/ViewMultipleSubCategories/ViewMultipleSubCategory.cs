using Humanizer;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ViewMultipleSubCategories
{
    public class ViewMultipleSubCategory
    {

        public record ViewMultipleCategoryResult
        {
            public int CategoryId { get; set; }
            public string Category_Description { get; set; }
            public int SubCategoryId { get; set; }
            public string Sub_Category_Description { get; set; }

        }

        public class ViewMultipleSubCategoryQuery : IRequest<Result>
        {
            public List<int> CategoryId { get; set; }
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

                //var subCategoryList = new Dictionary<int, List<ViewMultipleCategoryResult>>();

                //foreach(var category in request.CategoryId)
                //{
                //    var categoriesList = await _context.SubCategories
                //        .Include(s => s.Category)
                //        .Where(s => s.CategoryId == category)
                //        .Select(x => new ViewMultipleCategoryResult
                //        {
                //            CategoryId = x.CategoryId,
                //            Category_Description = x.Category.CategoryDescription,
                //            SubCategoryId = x.Id,
                //            Sub_Category_Description = x.SubCategoryDescription

                //        }).ToListAsync();

                //    subCategoryList.Add(category, categoriesList);

                //}

                var result = await _context.SubCategories
                    .Include(s => s.Category)
                    .Where(s => request.CategoryId.Contains(s.CategoryId))
                    .Select(x => new ViewMultipleCategoryResult
                    {
                        CategoryId = x.CategoryId,
                        Category_Description = x.Category.CategoryDescription,
                        SubCategoryId = x.Id,
                        Sub_Category_Description = x.SubCategoryDescription

                    }).ToListAsync();


                return Result.Success(result);
            }
        }
    }
}
