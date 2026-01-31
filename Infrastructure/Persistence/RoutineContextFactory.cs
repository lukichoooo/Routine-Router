using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class RoutineContextFactory : IDesignTimeDbContextFactory<RoutineContext>
{
    public RoutineContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RoutineContext>();
        optionsBuilder.UseSqlite("Data Source=events.db");
        return new RoutineContext(optionsBuilder.Options);
    }
}

