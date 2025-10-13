using System;

namespace CsvProcessor.Shared.Services
{
    public interface IMessagePublisher
    {
        void PublishFileMessage(Guid fileId, string filePath);
    }
}
