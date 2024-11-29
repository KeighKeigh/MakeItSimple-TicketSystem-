using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Dto.Pms_Form_Dto;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository.Pms_Form
{
    public class PmsFormRepository : IPmsFormRepository
    {
        private readonly MisDbContext context;

        public PmsFormRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async void CreatePmsForm(CreatePmsFormCommand pmsForm)
        {
            var add = new PmsForm
            {
               Form_Name = pmsForm.Form_Name,
               AddedBy = pmsForm.Added_By,

            };

            await context.PmsForms.AddAsync(add);
        }

        public async Task<bool> FormNameAlreadyExist(string Form)
        {
            return await context.PmsForms.AnyAsync(pf => pf.Form_Name == Form) ? true : false; 
        }
    }
}
