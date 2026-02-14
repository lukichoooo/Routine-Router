using Infrastructure.Persistence.Configs;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;


public class EventsContext(DbContextOptions<EventsContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfig());
    }
}

