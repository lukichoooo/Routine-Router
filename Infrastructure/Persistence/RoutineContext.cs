using System.Reflection;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;


public class RoutineContext : DbContext
{
    public DbSet<Event> Events { get; set; }

    public RoutineContext(DbContextOptions<RoutineContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

