using CsvProcessor.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

// ✅ Register DbContext
builder.Services.AddDbContext<CsvDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=CsvProcessorDb;Trusted_Connection=True;TrustServerCertificate=True;"));

// ✅ Register RabbitMQ ConnectionFactory as Singleton
builder.Services.AddSingleton(sp => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
    UserName = builder.Configuration["RabbitMQ:UserName"] ?? "myuser",
    Password = builder.Configuration["RabbitMQ:Password"] ?? "mypassword",
});

// ✅ Register CsvWorker background service
builder.Services.AddHostedService<CsvWorker>();

var host = builder.Build();
host.Run();
