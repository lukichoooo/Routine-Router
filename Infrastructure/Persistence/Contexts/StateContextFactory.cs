using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Contexts;


public class StateContextFactory : IDesignTimeDbContextFactory<StateContext>
{
    public StateContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StateContext>();
        optionsBuilder.UseSqlite("Data Source=routine-router.db");

        return new StateContext(optionsBuilder.Options);
    }
}

