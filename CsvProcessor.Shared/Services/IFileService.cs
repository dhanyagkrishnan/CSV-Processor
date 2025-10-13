using CsvProcessor.Shared.Models;

namespace CsvProcessor.Shared.Services;

public interface IFileService
{
    Task<Guid> StartProcessingFileAsync(Guid userId, string filePath);
}
