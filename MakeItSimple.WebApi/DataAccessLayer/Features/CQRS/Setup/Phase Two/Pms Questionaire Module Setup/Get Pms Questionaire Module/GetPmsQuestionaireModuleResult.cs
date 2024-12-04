namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Get_Pms_Questionaire_Module
{
    public partial class GetPmsQuestionaireModule 
    {
        public class GetPmsQuestionaireModuleResult
        {
            public int Id { get; set; }
            public string Questionaire_Module_Name { get; set; }
            public int PmsFormId { get; set; }
            public string Pms_Form_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Archived { get; set; }

        }
    }
}
