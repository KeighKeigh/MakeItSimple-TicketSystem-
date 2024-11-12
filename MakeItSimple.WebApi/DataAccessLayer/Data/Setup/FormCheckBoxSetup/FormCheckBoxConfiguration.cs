using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.FormCheckBoxSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormCheckBoxSetup
{
    public class FormCheckBoxConfiguration : IEntityTypeConfiguration<FormCheckBox>
    {
        public void Configure(EntityTypeBuilder<FormCheckBox> builder)
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
