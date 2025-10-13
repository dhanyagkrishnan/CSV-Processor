using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;
using System;
using System.Threading.Tasks;

namespace CsvProcessor.Shared.Services
{
    public class FileService : IFileService
    {
        private readonly CsvDbContext _dbContext;
        private readonly IMessagePublisher _publisher;

        public FileService(CsvDbContext dbContext, IMessagePublisher publisher)
        {
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task<Guid> StartProcessingFileAsync(Guid userId, string filePath)
        {
            var fileProcess = new FileProcess
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FilePath = filePath,
                Status = "NotStarted"
            };

            _dbContext.FileProcesses.Add(fileProcess);
            await _dbContext.SaveChangesAsync();

            // Publish message to queue
            _publisher.PublishFileMessage(fileProcess.Id, filePath);

            return fileProcess.Id;
        }
    }
}
