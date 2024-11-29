namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.GetQuestionCategories
{
    public partial class GetQuestionCategory
    {
        public record GetQuestionCategoryResult
        {
            public int Id { get; set; }
            public int FormId { get; set; }
            public string Form_Name { get; set; }
            public string Question_Category_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Active { get; set; }

        }
    }
}
