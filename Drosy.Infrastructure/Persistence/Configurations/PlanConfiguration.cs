using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p=> p.DaysOfWeek).IsRequired().HasConversion<int>();
        builder.Property(p => p.Type).IsRequired();
        builder.Property(p=> p.StartDate).IsRequired();
        builder.Property(p=> p.Status).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();
        
        builder.ToTable("Plans","EduManagement");
    }
}