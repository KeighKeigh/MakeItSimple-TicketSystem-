using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.GetForms
{
    public partial class GetForm
    {

        public class Handler : IRequestHandler<GetFormQuery, PagedList<GetFormResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetFormResult>> Handle(GetFormQuery request, CancellationToken cancellationToken)
            {

                var formResult =  _context.Forms
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .AsSplitQuery()
                    .Select(f => new GetFormResult
                    {
                        Id = f.Id,
                        Form_Name = f.Form_Name,
                        Added_By = f.AddedByUser.Fullname,
                        Created_At = f.CreatedAt,
                        Modified_By = f.ModifiedByUser.Fullname,
                        Updated_At = f.UpdatedAt,
                        IsActive = f.IsActive,

                    }).AsQueryable();


                if (!string.IsNullOrEmpty(request.Search))
                {
                    formResult = formResult
                        .Where(f => f.Form_Name.Contains(request.Search))
                        ;
                }

                if (request.Status is not null)
                {
                    formResult = formResult
                        .Where(f => f.IsActive == request.Status)
                        ;
                }


                return await PagedList<GetFormResult>.CreateAsync(formResult, request.PageNumber, request.PageSize);
            }
        }
    }
}
