using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.Pms_Form_Setup
{
    public class PmsFormConfiguration : IEntityTypeConfiguration<PmsForm>
    {
        public void Configure(EntityTypeBuilder<PmsForm> builder)
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
