using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;

public class NotStartedState : IFileProcessingState
{
    public async Task ProcessAsync(FileProcess file, CsvDbContext dbContext, CancellationToken cancellationToken)
    {
        file.Status = "InProgress";
        await dbContext.SaveChangesAsync(cancellationToken);

        var inProgress = new InProgressState();
        await inProgress.ProcessAsync(file, dbContext, cancellationToken);
    }
}
