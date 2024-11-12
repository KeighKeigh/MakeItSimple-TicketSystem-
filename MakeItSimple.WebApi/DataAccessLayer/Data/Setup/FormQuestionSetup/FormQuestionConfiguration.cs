using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormQuestionSetup
{
    public class FormQuestionConfiguration : IEntityTypeConfiguration<FormQuestion>
    {
        public void Configure(EntityTypeBuilder<FormQuestion> builder)
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
