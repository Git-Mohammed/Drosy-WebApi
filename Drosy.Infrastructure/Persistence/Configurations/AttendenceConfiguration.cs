using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    public class AttendenceConfiguration : IEntityTypeConfiguration<Attendence>
    {
        public void Configure(EntityTypeBuilder<Attendence> builder)
        {
            builder.HasKey(x => new { x.SessionId, x.StudentId});

            builder.Property(x => x.SessionId)
                 .IsRequired();

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.Property(x => x.Note)
                .IsRequired(false);

            builder.Property(x => x.Status).HasConversion<string>().IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.Student)
                .WithMany(x => x.Attendences)
                .HasForeignKey(x => x.StudentId)
                .IsRequired();

            builder.HasOne(x => x.Session)
              .WithMany(s => s.Attendences)
              .HasForeignKey(x => x.SessionId)
              .IsRequired();

            builder.ToTable("Attendences", "SessionManagement");

        }

    }
}
