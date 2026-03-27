using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpsPilot.Infrastructure.Persistence;

public class OpsPilotDbContextFactory : IDesignTimeDbContextFactory<OpsPilotDbContext>
{
    public OpsPilotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OpsPilotDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OpsPilotDb;Trusted_Connection=True;TrustServerCertificate=True;");
        return new OpsPilotDbContext(optionsBuilder.Options);
    }
}