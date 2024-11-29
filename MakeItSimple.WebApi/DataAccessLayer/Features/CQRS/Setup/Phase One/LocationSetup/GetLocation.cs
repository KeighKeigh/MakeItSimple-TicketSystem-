﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CompanySetup.GetCompany;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.LocationSetup
{
    public class GetLocation
    {
        public class GetLocationResult
        {

            public int Location_No { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            //public string SubUnit_Code { get; set; }
            //public string SubUnit_Name { get; set; }    
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }

            public ICollection<SubUnit> SubUnits { get; set; }

            public class SubUnit
            {
                public int LocationId { get; set; }
                public int? SubUnitId { get; set; }
                public string SubUnit_Code { get; set; }
                public string SubUnit_Name { get; set; }
            }
        }

        public class GetLocationQuery : UserParams, IRequest<PagedList<GetLocationResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }

        public class IHandler : IRequestHandler<GetLocationQuery, PagedList<GetLocationResult>>
        {
            private readonly MisDbContext _context;

            public IHandler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetLocationResult>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Location> locationQuery = _context.Locations.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser)/*.Include(x => x.SubUnits)*/;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    locationQuery = locationQuery.Where(x => x.LocationCode.Contains(request.Search) || x.LocationName.Contains(request.Search));

                }


                if (request.Status != null)
                {
                    locationQuery = locationQuery.Where(x => x.IsActive == request.Status);
                }

                var results = locationQuery.GroupBy(x => x.LocationNo).Select(x => new GetLocationResult
                {
                    //Id = x.Id,
                    Location_No = x.Key,

                    Location_Code = x.First().LocationCode,
                    Location_Name = x.First().LocationName,
                    Added_By = x.First().AddedByUser.Fullname,
                    //SubUnit_Code = x.SubUnit.SubUnitCode,
                    //SubUnit_Name = x.SubUnit.SubUnitName,
                    Created_At = x.First().CreatedAt,
                    Modified_By = x.First().ModifiedByUser.Fullname,
                    Updated_At = x.First().UpdatedAt,
                    Sync_Status = x.First().SyncStatus,
                    SyncDate = x.First().SyncDate,
                    SubUnits = x.Select(x => new GetLocationResult.SubUnit
                    {
                        LocationId = x.Id,
                        SubUnitId = x.SubUnitId,
                        SubUnit_Code = x.SubUnit.SubUnitCode,
                        SubUnit_Name = x.SubUnit.SubUnitName,


                    }).ToList(),

                });

                return await PagedList<GetLocationResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
