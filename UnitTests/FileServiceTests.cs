using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;
using CsvProcessor.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;

public class FileServiceTests
{
    [Fact]
    public async Task StartProcessingFileAsync_CreatesFileProcess()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CsvDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        using var dbContext = new CsvDbContext(options);

        // ✅ Mock IMessagePublisher instead of ConnectionFactory
        var mockPublisher = new Mock<IMessagePublisher>();

        // Create FileService with mock dependency
        var fileService = new FileService(dbContext, mockPublisher.Object);

        var userId = Guid.NewGuid();
        string filePath = "test.csv";

        // Act
        var fileProcessId = await fileService.StartProcessingFileAsync(userId, filePath);

        // Assert
        var file = await dbContext.FileProcesses.FirstOrDefaultAsync();
        Assert.NotNull(file);
        Assert.Equal(filePath, file.FilePath);
        Assert.Equal("NotStarted", file.Status);  // ✅ changed based on new FileService
        mockPublisher.Verify(p => p.PublishFileMessage(fileProcessId, filePath), Times.Once);
    }
}
