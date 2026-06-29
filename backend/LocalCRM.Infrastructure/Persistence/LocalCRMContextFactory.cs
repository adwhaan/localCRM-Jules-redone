using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LocalCRM.Infrastructure.Persistence;

public class LocalCRMContextFactory : IDesignTimeDbContextFactory<LocalCRMContext>
{
    public LocalCRMContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalCRMContext>();
        optionsBuilder.UseSqlite("Data Source=localcrm.db");

        return new LocalCRMContext(optionsBuilder.Options);
    }
}
