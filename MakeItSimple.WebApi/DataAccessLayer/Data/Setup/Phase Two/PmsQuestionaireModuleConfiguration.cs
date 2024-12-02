using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two
{
    public class PmsQuestionaireModuleConfiguration : IEntityTypeConfiguration<PmsQuestionaireModule>
    {
        public void Configure(EntityTypeBuilder<PmsQuestionaireModule> builder)
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
