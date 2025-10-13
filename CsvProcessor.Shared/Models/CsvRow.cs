namespace CsvProcessor.Shared.Models;

public class CsvRow
{
    public Guid Id { get; set; }
    public Guid FileProcessId { get; set; }
    public string Column1 { get; set; } = null!;
    public string Column2 { get; set; } = null!;
    public string RowData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
