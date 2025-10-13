using CsvProcessor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CsvProcessor.Shared.Data;

public class CsvDbContext : DbContext
{
    public CsvDbContext(DbContextOptions<CsvDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<FileProcess> FileProcesses => Set<FileProcess>();
    public DbSet<CsvRow> CsvRows => Set<CsvRow>();
}
