using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Get_Pms_Question
{
    public partial class GetPmsQuestion
    {

        public class Handler : IRequestHandler<GetPmsQuestionQuery, PagedList<GetPmsQuestionResult>>
        {
            private readonly MisDbContext context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {

                this.context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<PagedList<GetPmsQuestionResult>> Handle(GetPmsQuestionQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsQuestionaire> query = context.PmsQuestionaires
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .AsSplitQuery();

                if (!string.IsNullOrEmpty(request.Orders))
                    query = unitOfWork.PmsQuestion.OrdersPmsForm(request.Orders);

                if (request.Is_Archived is not null)
                    query = query.Where(q => unitOfWork.PmsQuestion.ArchivedPmsForm(request.Is_Archived).Contains(q));

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(q => unitOfWork.PmsQuestion.SearchPmsForm(request.Search).Contains(q));


                var results = query
                    .Select(x => new GetPmsQuestionResult
                    {
                       Id = x.Id,
                       PmsQuestionModules = x.QuestionTransactionIds
                       .Select(x => new GetPmsQuestionResult.PmsQuestionModule
                       {

                           Question_Transaction_Id = x.Id,
                           Id = x.PmsQuestionaireModuleId,
                           Question_Module_Name = x.PmsQuestionaireModule.QuestionaireModuleName,

                       }).ToList(),
                       Question = x.Question,
                       QuestionType = x.QuestionType,
                       PmsQuestionTypes = x.PmsQuestionTypes
                       .Select(x => new GetPmsQuestionResult.PmsQuestionType
                       {
                           Id = x.Id,
                           Description = x.Description,

                       }).ToList(),
                       Added_By = x.AddedByUser.Fullname,
                       Created_At = x.CreatedAt.Date,
                       Modified_By = x.ModifiedByUser.Fullname,
                       Updated_At = x.UpdatedAt.Value.Date,

                    });

                return await PagedList<GetPmsQuestionResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
