namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Get_Pms_Question
{
    public partial class GetPmsQuestion
    {
        public record GetPmsQuestionResult
        {

            public int Id  { get; set; }
            public string Question { get; set; }
            public List<PmsQuestionModule> PmsQuestionModules { get; set; }
            public string QuestionType { get; set; }
            public List<PmsQuestionType> PmsQuestionTypes { get; set;}
            public record PmsQuestionModule
            {
                public int Id { get; set; }
                public int Question_Transaction_Id { get; set; }
                public string Question_Module_Name { get; set; }
            }
            public record PmsQuestionType
            {
                public int Id { get; set; }
                public string Description { get; set; }
            } 

            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Active { get; set; }
        }
    }
}
