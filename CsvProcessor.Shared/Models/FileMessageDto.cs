namespace CsvProcessor.Shared.Models
{
    public class FileMessageDto
    {
        public Guid FileId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
