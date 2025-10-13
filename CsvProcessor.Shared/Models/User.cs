namespace CsvProcessor.Shared.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
