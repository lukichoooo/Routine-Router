using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Contexts;


public class EntitiesContextFactory : IDesignTimeDbContextFactory<EntitiesContext>
{
    public EntitiesContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EntitiesContext>();
        optionsBuilder.UseSqlite("Data Source=entities.db");

        return new EntitiesContext(optionsBuilder.Options);
    }
}

