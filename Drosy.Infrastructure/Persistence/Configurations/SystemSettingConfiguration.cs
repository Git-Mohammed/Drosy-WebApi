using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.ToTable("SystemSettings", "Configuration");

            builder.Property(x => x.WebName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.DefaultCurrency)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(x => x.LogoPath)
                   .HasMaxLength(255);

            builder.HasData(new SystemSetting { Id = 1, DefaultCurrency = "USD", WebName = "Drosy" });
        }
    }
}
