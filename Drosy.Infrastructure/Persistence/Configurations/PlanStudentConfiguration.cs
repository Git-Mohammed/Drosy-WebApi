using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal partial class PlanStudentConfiguration : IEntityTypeConfiguration<PlanStudent>
    {
        public void Configure(EntityTypeBuilder<PlanStudent> builder)
        {
            builder.HasKey(x => new { x.PlanId,x.StudentId});

            builder.Property(x => x.PlanId)
               .IsRequired();

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.Property(x => x.Notes)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .IsRequired();

            builder.HasOne(x => x.Plan)
              .WithMany()
              .HasForeignKey(x => x.PlanId)
              .IsRequired();

            builder.ToTable("PlanStudents");
        }

    }
}