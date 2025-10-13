namespace CsvProcessor.Shared.Models;

public class FileProcess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FilePath { get; set; } = null!;
    public string Status { get; set; } = "InProgress";
    public long? TotalRows { get; set; }
    public long ProcessedRows { get; set; }
    public int LastProcessedRowIndex { get; set; } = 0; // Track last processed row

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
