using Drosy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
           builder.HasKey(x => x.Id);
            
           builder.Property(x => x.Title)
                .IsRequired();

            builder.Property(x => x.ExcepectedDate)
                .IsRequired();

            builder.Property(x => x.StartTime)
            .IsRequired();

            builder.Property(x => x.EndTime)
            .IsRequired();

            // Relationships
            builder.HasOne(x => x.Plan)
                .WithMany(p => p.Sessions)
                .HasForeignKey(x => x.PlanId);

        }

    }
}
