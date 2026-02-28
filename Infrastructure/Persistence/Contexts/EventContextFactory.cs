using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Contexts;


public class EventContextFactory : IDesignTimeDbContextFactory<EventContext>
{
    public EventContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventContext>();
        optionsBuilder.UseSqlite("Data Source=routine-router.db");

        optionsBuilder.EnableSensitiveDataLogging();

        return new EventContext(optionsBuilder.Options);
    }
}

