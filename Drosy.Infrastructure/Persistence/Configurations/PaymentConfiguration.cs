using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration: IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("Payments","Finance");



        #region Relations

        builder.HasOne(p=> p.Plan)
            .WithMany(p=> p.Payments)
            .HasForeignKey(p=> p.PlanId);
        
        builder.HasOne(p=> p.Student)
            .WithMany(s=> s.Payments)
            .HasForeignKey(p=> p.StudentId);

        #endregion
    }
}