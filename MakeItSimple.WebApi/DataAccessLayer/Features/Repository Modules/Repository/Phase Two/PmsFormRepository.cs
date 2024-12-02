using DocumentFormat.OpenXml.Office2010.Excel;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Enumerator;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form.UpdatePmsForm;
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

        public async Task CreatePmsForm(CreatePmsFormCommand pmsForm)
        {
            var add = new PmsForm
            {
               Form_Name = pmsForm.Form_Name,
               AddedBy = pmsForm.Added_By,
            };

            await context.PmsForms.AddAsync(add);
        }

        public async Task<bool> FormNameAlreadyExist(string form)
        {
            return await context.PmsForms.AnyAsync(pf => pf.Form_Name == form); 
        }

        public IQueryable<PmsForm> SearchPmsForm(string search)
        {
            return  context.PmsForms.Where(x => x.Form_Name.ToLower().Contains(search));
        }
        public  IQueryable<PmsForm> ArchivedPmsForm(bool? is_Archived)
        {
            return context.PmsForms.Where(q => q.IsActive == is_Archived);
        }

        public IQueryable<PmsForm> OrdersPmsForm(string order_By)
        {
            var query = context.PmsForms.AsQueryable();

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.Id);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.Id);
                    break;

                default:
                    query = query.OrderBy(x => x.Form_Name);
                    break;
            }

            return query;
        }

        public async Task<PmsForm> PmsFormIdNotExist(int id)
        {
            return await context.PmsForms.FindAsync(id);
        }

        public Task UpdatePmsForm(UpdatePmsFormCommand pmsForm)
        {

            context.ChangeTracker.Clear();

            var updateForm = new PmsForm
            {
                Id = pmsForm.Id,
                Form_Name = pmsForm.Form_Name,
                ModifiedBy = pmsForm.Modified_By,
                UpdatedAt = DateTime.Now,
            };


            context.PmsForms.Attach(updateForm);
            context.Entry(updateForm).Property(x => x.Form_Name).IsModified = true;
            context.Entry(updateForm).Property(x => x.ModifiedBy).IsModified = true;
            context.Entry(updateForm).Property(x => x.UpdatedAt).IsModified = true;

            return Task.CompletedTask;
        }

        public Task UpdateStatus(int id , bool status)
        {
            context.ChangeTracker.Clear();

            var updateForm = new PmsForm
            {
                Id= id,
                IsActive = !status,
            };

            context.PmsForms.Attach(updateForm);
            context.Entry(updateForm).Property(x => x.IsActive).IsModified = true;

            return Task.CompletedTask;
        }
    }
}
