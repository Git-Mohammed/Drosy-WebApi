using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Regions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasMany(x => x.Cities)
               .WithOne(x => x.Region)
               .HasForeignKey(x => x.RegionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Region { Id = 1, Name = "Ma’rib" },
            new Region { Id = 2, Name = "Sana’a" },
            new Region { Id = 3, Name = "Aden" },
            new Region { Id = 4, Name = "Ta’izz" },
            new Region { Id = 5, Name = "Hadramawt" },
            new Region { Id = 6, Name = "Al Hudaydah" },
            new Region { Id = 7, Name = "Ibb" },
            new Region { Id = 8, Name = "Shabwah" },
            new Region { Id = 9, Name = "Al Mahrah" },
            new Region { Id = 10, Name = "Al Jawf" },
            new Region { Id = 11, Name = "Amran" },
            new Region { Id = 12, Name = "Dhamar" },
            new Region { Id = 13, Name = "Raymah" },
            new Region { Id = 14, Name = "Sa’dah" },
            new Region { Id = 15, Name = "Lahij" },
            new Region { Id = 16, Name = "Al Bayda" },
            new Region { Id = 17, Name = "Abyan" },
            new Region { Id = 18, Name = "Al Dhale’e" },
            new Region { Id = 19, Name = "Socotra" },
            new Region { Id = 20, Name = "Hajjah" },
            new Region { Id = 21, Name = "Mahwit" },
            new Region { Id = 22, Name = "Sana’a City" }
        );
    }

}
