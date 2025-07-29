using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Table configuration: mapped to "Payments" table under "Finance" schema
        builder.ToTable("Payments", "Finance");

        // Define precision and requirements for the Amount field
        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)") // Stored as numeric type in DB
            .IsRequired()
            .HasComment("The monetary amount paid by the student");

        // Store enum value as integer
        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Indicates how the payment was made (1 = Cash, 2 = BankTransfer, 3 = CreditCard, 4 = MobilePayment, 5 = Scholarship, 6 = Other)");

        // Optional field to hold additional notes
        builder.Property(p => p.Notes)
            .HasMaxLength(500)
            .HasComment("Optional notes or remarks attached to the payment");

        // Track when the payment record was created
        builder.Property(p => p.CreatedAt)
            .HasComment("Timestamp indicating when the payment was created");

        #region Relations

        // Link to Plan entity
        builder.HasOne(p => p.Plan)
            .WithMany(pl => pl.Payments)
            .HasForeignKey(p => p.PlanId);
        // Link to Student entity
        builder.HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentId);
        #endregion
    }
}
