using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace WorkerService
{
    public class CsvWorker : BackgroundService
    {
        private readonly ILogger<CsvWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;

        public CsvWorker(ILogger<CsvWorker> logger, IServiceScopeFactory scopeFactory, ConnectionFactory factory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _factory = factory;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "csv_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<FileMessage>(Encoding.UTF8.GetString(body));

                    if (message != null)
                    {
                        _logger.LogInformation($"📂 Received message for file: {message.FilePath}");

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<CsvDbContext>();

                        var file = await db.FileProcesses.FindAsync(message.FileProcessId);
                        if (file != null)
                        {
                            IFileProcessingState state = file.Status switch
                            {
                                "NotStarted" => new NotStartedState(),
                                "InProgress" => new InProgressState(),
                                _ => null
                            };
                            //if (file.Status == "NotStarted")
                            //{
                            //    file.Status = "InProgress";
                            //    file.UpdatedAt = DateTime.UtcNow;
                            //    await db.SaveChangesAsync();
                            //}
                            if (state != null)
                          {
                                await state.ProcessAsync(file, db, stoppingToken);
                               
                           }
                        }
                    }

                    // ✅ Acknowledge message only after processing
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error processing CSV message");

                   
                }
            };

            // Start consuming messages
            channel.BasicConsume(queue: "csv_queue", autoAck: false, consumer: consumer);

            _logger.LogInformation("✅ CsvWorker is now listening for messages...");
            return Task.CompletedTask;
        }


        private async Task ProcessFileAsync(CsvDbContext dbContext, Guid fileProcessId, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠ File not found: {filePath}");

                    // 🔹 Update file status to "Error"
                    var missingFile = await dbContext.FileProcesses.FindAsync(fileProcessId);
                    if (missingFile != null)
                    {
                        missingFile.Status = "Error";
                        missingFile.UpdatedAt = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                    }

                    return;
                }

                // 🔹 Read CSV lines
                var lines = await File.ReadAllLinesAsync(filePath);

                // 🔹 Skip header and parse rows
                var csvRows = lines.Skip(1).Select(line =>
                {
                    var parts = line.Split(',');
                    return new CsvRow
                    {
                        Id = Guid.NewGuid(),
                        FileProcessId = fileProcessId,
                        Column1 = parts.ElementAtOrDefault(0),
                        Column2 = parts.ElementAtOrDefault(1)
                    };
                }).ToList();

                // 🔹 Save all rows to database
                await dbContext.CsvRows.AddRangeAsync(csvRows);

                // 🔹 Update FileProcess table
                var fileProcess = await dbContext.FileProcesses.FindAsync(fileProcessId);
                if (fileProcess != null)
                {
                    fileProcess.Status = "Completed";
                    fileProcess.TotalRows = lines.Length - 1; // Exclude header
                    fileProcess.ProcessedRows = csvRows.Count;
                    fileProcess.UpdatedAt = DateTime.Now;
                }

                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"✅ Processed {csvRows.Count} rows from {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error processing file {filePath}");

                // 🔹 On error, mark as Failed
                var fileProcess = await dbContext.FileProcesses.FindAsync(fileProcessId);
                if (fileProcess != null)
                {
                    fileProcess.Status = "Error";
                    fileProcess.UpdatedAt = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        private class FileMessage
        {
            public Guid FileProcessId { get; set; }
            public string FilePath { get; set; } = string.Empty;
        }
    }
}
