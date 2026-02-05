using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Contexts;


public class EventsContextFactory : IDesignTimeDbContextFactory<EventsContext>
{
    public EventsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventsContext>();
        optionsBuilder.UseSqlite("Data Source=events.db");


        optionsBuilder.EnableSensitiveDataLogging();

        return new EventsContext(optionsBuilder.Options);
    }
}

