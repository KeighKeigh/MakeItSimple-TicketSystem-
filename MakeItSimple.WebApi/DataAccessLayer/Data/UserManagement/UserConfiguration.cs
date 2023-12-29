﻿using MakeItSimple.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(new User
            {
                Id = Guid.Parse("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                Fullname = "Admin",
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "$2a$12$ihvpKbpvdRfZLXz.tZKFEulxnTg1tiS11T/MbpufId3rzXoCMW2OK",
                UserRoleId = 1,
                IsPasswordChange = true,
            });


            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);
           
            
            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        }



    }
}
