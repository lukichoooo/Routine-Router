using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;


public sealed class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        // ---------- PRIMARY KEY ----------
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        // ---------- REQUIRED FIELDS ----------
        builder.Property(x => x.AggregateIdType)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.EventType)
               .IsRequired()
               .HasMaxLength(300);

        // ---------- INDEXES ----------

        builder.HasIndex(x => new { x.AggregateId, x.Version, x.EventType })
               .IsUnique();

        // ---------- ORDERING ----------
        builder.HasIndex(x => x.Id)
               .IsUnique();
    }
}

