using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Unity.Microsoft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();

// --- DB connection from env variable or appsettings.json
builder.Services.AddDbContext<CsvDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- RabbitMQ connection from env variables
// --- RabbitMQ configuration (connection factory)
builder.Services.AddSingleton(sp => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
    UserName = builder.Configuration["RabbitMQ:UserName"] ?? "myuser",
    Password = builder.Configuration["RabbitMQ:Password"] ?? "mypassword",
});

// ✅ Register message publisher (new class)
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

// ✅ Register FileService (depends on IMessagePublisher now)
builder.Services.AddScoped<IFileService, FileService>();

builder.Host.UseUnityServiceProvider();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();
app.MapRazorPages();
app.Run();
