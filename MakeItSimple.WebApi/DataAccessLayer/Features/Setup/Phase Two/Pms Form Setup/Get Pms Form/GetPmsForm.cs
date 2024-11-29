using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form
{
    public class GetPmsForm
    {
        public class GetPmsFormResult
        {
            public int Id { get; set; }
            public string Form_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Archived { get; set; }

        }

        public class GetPmsFormQuery : UserParams , IRequest<PagedList<GetPmsFormResult>> 
        {
            public string Search {  get; set; }
            public bool ? Is_Archived { get; set; }
            public string Order_By { get; set; }

        }

        public class Handler : IRequestHandler<GetPmsFormQuery, PagedList<GetPmsFormResult>>
        {
            private readonly MisDbContext context;
            private readonly IUnitOfWork unitOfWork;


            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                this.context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<PagedList<GetPmsFormResult>> Handle(GetPmsFormQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsForm> query = context.PmsForms
                     .AsNoTrackingWithIdentityResolution()
                     .Include(q => q.AddedByUser)
                     .Include(q => q.ModifiedByUser)
                     .AsSplitQuery();

                if(!string.IsNullOrEmpty(request.Search))
                    query = await unitOfWork.PmsForm.GetPmsForm(request.Search);

                var result = query
                    .Select(q => new GetPmsFormResult
                    {

                    });

                return await PagedList<GetPmsFormResult>.CreateAsync(result,request.PageNumber,request.PageSize);   
            }
        }
    }
}
