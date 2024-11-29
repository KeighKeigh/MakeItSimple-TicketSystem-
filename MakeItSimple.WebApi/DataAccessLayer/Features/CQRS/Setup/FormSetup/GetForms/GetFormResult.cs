namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.GetForms
{
    public partial class GetForm
    {
        public class GetFormResult
        {
            public int Id { get; set; }
            public string Form_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool IsActive { get; set; }

        }
    }
}
