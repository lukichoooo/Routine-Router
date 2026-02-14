using Domain.Entities.Schedules;
using Domain.Entities.Users;
using Infrastructure.Persistence.Configs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;


public class EntitiesContext(DbContextOptions<EntitiesContext> options) : DbContext(options)
{
    public DbSet<ChecklistState> Checklists { get; set; }
    public DbSet<UserState> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ChecklistStateConfig());
        modelBuilder.ApplyConfiguration(new UserStateConfig());
        modelBuilder.ApplyConfiguration(new TaskEntityConfig());
    }
}


