using System.Reflection;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;


public class EventsContext : DbContext
{
    public DbSet<Event> Events { get; set; }

    public EventsContext(DbContextOptions<EventsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

