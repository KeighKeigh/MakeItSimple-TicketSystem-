using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.FormDropdownSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormDropdownSetup
{
    public class FormDropdownConfiguration : IEntityTypeConfiguration<FormDropdown>
    {
        public void Configure(EntityTypeBuilder<FormDropdown> builder)
        {

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
