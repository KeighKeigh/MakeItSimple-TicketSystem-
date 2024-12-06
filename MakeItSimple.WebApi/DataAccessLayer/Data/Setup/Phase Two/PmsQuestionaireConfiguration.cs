using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two
{
    public class PmsQuestionaireConfiguration : IEntityTypeConfiguration<PmsQuestionaire>
    {
        public void Configure(EntityTypeBuilder<PmsQuestionaire> builder)
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
