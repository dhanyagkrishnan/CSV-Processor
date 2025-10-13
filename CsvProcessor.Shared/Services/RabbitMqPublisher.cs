using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace CsvProcessor.Shared.Services
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(ConnectionFactory factory)
        {
            _factory = factory;
        }

        public void PublishFileMessage(Guid fileId, string filePath)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("csv_queue", durable: true, exclusive: false, autoDelete: false);

            var message = new { FileProcessId = fileId, FilePath = filePath };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", "csv_queue", null, body);
        }
    }
}
