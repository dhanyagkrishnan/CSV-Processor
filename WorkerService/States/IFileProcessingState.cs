using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;

public interface IFileProcessingState
{
    Task ProcessAsync(FileProcess file, CsvDbContext dbContext, CancellationToken cancellationToken);
}
