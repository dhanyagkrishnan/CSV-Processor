using CsvProcessor.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

// ✅ Register DbContext
builder.Services.AddDbContext<CsvDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=CsvProcessorDb;Trusted_Connection=True;TrustServerCertificate=True;"));

// ✅ Register RabbitMQ ConnectionFactory as Singleton
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "myuser",
    Password = "mypassword",
    DispatchConsumersAsync = false // use true if using async consumer
});

// ✅ Register CsvWorker background service
builder.Services.AddHostedService<CsvWorker>();

var host = builder.Build();
host.Run();
