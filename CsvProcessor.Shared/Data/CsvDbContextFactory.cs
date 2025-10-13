using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CsvProcessor.Shared.Data
{
    public class CsvDbContextFactory : IDesignTimeDbContextFactory<CsvDbContext>
    {
        public CsvDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CsvDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=CsvProcessorDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new CsvDbContext(optionsBuilder.Options);
        }
    }
}
