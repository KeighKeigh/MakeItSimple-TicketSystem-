using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.AddForms
{
    public partial class AddNewForm
    {
        public class AddNewFormCommand : IRequest<Result>
        {
            public int? Id { get; set; }
            public string Form_Name { get; set; }
            public Guid Added_By { get; set; }
            public Guid Modified_By { get; set; }

        }
    }
}
