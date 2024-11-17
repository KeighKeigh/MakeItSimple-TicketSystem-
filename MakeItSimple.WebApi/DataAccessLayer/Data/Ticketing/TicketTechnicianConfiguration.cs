using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketTechnicianConfiguration : IEntityTypeConfiguration<TicketTechnician>
    {
        public void Configure(EntityTypeBuilder<TicketTechnician> builder)
        {

            builder.HasOne(u => u.TechnicianByUser)
           .WithMany()
           .HasForeignKey(u => u.TechnicianBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
