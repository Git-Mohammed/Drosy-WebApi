using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations;

public class PlanDayConfiguration : IEntityTypeConfiguration<PlanDay>
{
    public void Configure(EntityTypeBuilder<PlanDay> builder)
    {
        builder.HasKey(pd => pd.Id);
        builder.Property(pd => pd.Day).IsRequired();
        builder.Property(pd => pd.StartSession).IsRequired();
        builder.Property(pd => pd.EndSession).IsRequired();
        
        builder.ToTable("PlanDays", "EduManagement");
    }
}