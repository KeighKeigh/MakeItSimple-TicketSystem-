using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Get_Pms_Questionaire_Module
{
    public partial class GetPmsQuestionaireModule 
    {

        public class Handler : IRequestHandler<GetPmsQuestionaireModuleQuery, PagedList<GetPmsQuestionaireModuleResult>>
        {
            private readonly IUnitOfWork unitOfWork;
            private readonly MisDbContext context;

            public Handler(IUnitOfWork unitOfWork, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                this.context = context;
            }

            public async Task<PagedList<GetPmsQuestionaireModuleResult>> Handle(GetPmsQuestionaireModuleQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsQuestionaireModule> query = context.PmsQuestionaireModules
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.PmsForm)
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Orders))
                    query = unitOfWork.PmsQuestionaireModules.OrdersPmsForm(request.Orders);

                if (request.Is_Archived is not null)
                    query = query.Where(q => unitOfWork.PmsQuestionaireModules.ArchivedPmsForm(request.Is_Archived).Contains(q));

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(q => unitOfWork.PmsQuestionaireModules.SearchPmsForm(request.Search).Contains(q));


                var result = query
                    .Select(q => new GetPmsQuestionaireModuleResult
                    {
                        Id = q.Id,
                        Questionaire_Module_Name = q.QuestionaireModuleName,
                        PmsFormId = q.PmsFormId,
                        Pms_Form_Name = q.PmsForm.Form_Name,
                        Added_By = q.AddedByUser.Fullname,
                        Created_At = q.CreatedAt.Date,
                        Modified_By = q.ModifiedByUser.Fullname,
                        Updated_At = q.UpdatedAt,
                        Is_Archived = q.IsActive,
                    });

                return await PagedList<GetPmsQuestionaireModuleResult>.CreateAsync(result,request.PageNumber,request.PageSize);
            }
        }
    }
}
