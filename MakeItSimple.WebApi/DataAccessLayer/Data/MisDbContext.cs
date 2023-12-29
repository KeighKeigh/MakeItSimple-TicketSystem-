﻿using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ChannelSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CompanySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.DepartmentSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.LocationSetup;

using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace MakeItSimple.WebApi.DataAccessLayer.Data
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<AccountTitle> AccountTitles { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<SubUnit> SubUnits { get; set; }
        public virtual DbSet<Channel> Channels {  get; set; }
        public virtual DbSet<ChannelUser> ChannelUsers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTitleConfiguration());
            modelBuilder.ApplyConfiguration(new SubUnitConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConfiguration());

        }


    }
}
