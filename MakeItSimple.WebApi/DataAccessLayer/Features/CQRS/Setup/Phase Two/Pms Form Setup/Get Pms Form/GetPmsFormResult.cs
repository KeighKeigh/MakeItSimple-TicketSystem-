﻿namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form
{
    public partial class GetPmsForm
    {
        public record GetPmsFormResult
        {
            public int Id { get; set; }
            public string Form_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Archived { get; set; }

            public List<PmsQuestionModule> PmsQuestionModules { get; set; }
            public record PmsQuestionModule
            {
                public int Id { get; set; }
                public string Question_Module_Name { get; set; }
            }


        }
    }
}
