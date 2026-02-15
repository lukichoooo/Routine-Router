using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;


public sealed class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        // ---------- PRIMARY KEY ----------
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .ValueGeneratedOnAdd();

        // ---------- REQUIRED FIELDS ----------
        builder.Property(e => e.AggregateIdType)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.AggregateId)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.EventType)
               .IsRequired()
               .HasMaxLength(300);

        builder.Property(e => e.Payload)
               .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP") // SQLite 
               .ValueGeneratedOnAdd();

        // ---------- INDEXES ----------

        builder.HasIndex(e => new { e.AggregateId, e.Version, e.EventType })
               .IsUnique();

        // ---------- ORDERING ----------
        builder.HasIndex(e => e.Version);
    }
}

