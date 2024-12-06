using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ApproverSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ChannelSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CompanySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.DepartmentSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormCheckBoxSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormDropdownSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormQuestionSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.LocationSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.Pms_Form_Setup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.QuestionCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ReceiverSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.UnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Data.UserManagement;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.FormCheckBoxSetup;
using MakeItSimple.WebApi.Models.Setup.FormDropdownSetup;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MakeItSimple.WebApi.Models.Setup.ReceiverSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.DataContext
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        // Setup

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<AccountTitle> AccountTitles { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<SubUnit> SubUnits { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<ChannelUser> ChannelUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Approver> Approvers { get; set; }

        //Ticketing

        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }
        public virtual DbSet<TicketConcern> TicketConcerns { get; set; }
        public virtual DbSet<TransferTicketConcern> TransferTicketConcerns { get; set; }
        public virtual DbSet<ClosingTicket> ClosingTickets { get; set; }
        public virtual DbSet<ApproverTicketing> ApproverTicketings { get; set; }
        public virtual DbSet<TicketHistory> TicketHistories { get; set; }
        public virtual DbSet<Receiver> Receivers { get; set; }
        public virtual DbSet<RequestConcern> RequestConcerns { get; set; }
        public virtual DbSet<TicketComment> TicketComments { get; set; }
        public virtual DbSet<TicketCommentView> TicketCommentViews { get; set; }
        public virtual DbSet<TicketTransactionNotification> TicketTransactionNotifications { get; set; }
        public virtual DbSet<TicketOnHold> TicketOnHolds { get; set; }
        public virtual DbSet<TicketCategory> TicketCategories { get; set; }

        public virtual DbSet<TicketSubCategory> TicketSubCategories {  get; set; } 
        public virtual DbSet<TicketTechnician> TicketTechnicians { get; set; }

        //Phase 2

        public virtual DbSet<PmsForm> PmsForms { get; set; }
        public virtual DbSet<PmsQuestionaireModule> PmsQuestionaireModules { get; set; }
        public virtual DbSet<PmsQuestionaire> PmsQuestionaires { get; set; }
        public virtual DbSet<QuestionTransactionId> QuestionTransactionIds { get; set; }




        //Phase 3

        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }
        public virtual DbSet<FormQuestion> FormQuestions { get; set; }
        public virtual DbSet<FormCheckBox> FormCheckBoxes { get; set; }
        public virtual DbSet<FormDropdown> FormDropdowns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new BusinessUnitConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTitleConfiguration());
            modelBuilder.ApplyConfiguration(new SubUnitConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new TransferTicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverTicketingConfiguration());
            modelBuilder.ApplyConfiguration(new TicketHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ClosingTicketConfiguration());
            modelBuilder.ApplyConfiguration(new RequestConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ReceiverConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentViewConfiguration());
            modelBuilder.ApplyConfiguration(new TicketTransactionNotificationConfiguration());
            modelBuilder.ApplyConfiguration(new TicketOnHoldConfiguration());
            modelBuilder.ApplyConfiguration(new TicketTechnicianConfiguration());

            //Phase 2

            modelBuilder.ApplyConfiguration(new PmsFormConfiguration());
            modelBuilder.ApplyConfiguration(new PmsQuestionaireModuleConfiguration());
            modelBuilder.ApplyConfiguration(new PmsQuestionaireConfiguration());



            //Phase 3

            modelBuilder.ApplyConfiguration(new FormConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new FormQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new FormCheckBoxConfiguration());
            modelBuilder.ApplyConfiguration(new FormDropdownConfiguration());

        }





    }
}
