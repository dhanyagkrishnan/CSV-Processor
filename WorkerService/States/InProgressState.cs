using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;

public class InProgressState : IFileProcessingState
{
    public async Task ProcessAsync(FileProcess file, CsvDbContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(file.FilePath))
            {
                // ❌ File not found
                file.Status = "Error";
                file.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            using var reader = new StreamReader(file.FilePath);
            bool isFirstRow = true;
            int currentRow = 0;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                // ✅ Skip CSV header
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }

                currentRow++;

                // ✅ Skip already processed rows (resume functionality)
                if (currentRow <= file.LastProcessedRowIndex)
                    continue;

                // ✅ Insert CSV row
                dbContext.CsvRows.Add(new CsvRow
                {
                    Id = Guid.NewGuid(),
                    FileProcessId = file.Id,
                    RowData = line
                });

                file.ProcessedRows++;

                // ✅ Save progress every 100 rows
                if (currentRow % 100 == 0)
                {
                    file.LastProcessedRowIndex = currentRow;
                    file.UpdatedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            // ✅ Final save at the end
            file.LastProcessedRowIndex = currentRow;
            file.Status = "Completed";
            file.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // ❌ On any error, mark file as failed
            file.Status = "Error";
            file.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
