using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using RabbitMQ.Client;
using SmartClinic.AppointmentScheduling.Infrastructure.Persistence;
using SmartClinic.BuildingBlocks.BackgroundJobs;
using SmartClinic.BuildingBlocks.Messaging;
using SmartClinic.BuildingBlocks.Outbox;
using SmartClinic.PatientManagement.Infrastructure.Persistence;
using SmartClinic.PrescriptionManagement.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

// 1. Register DbContexts
// Using InMemory for demonstration/verification purposes as per instructions.
// In production, these would use SQL Server connection strings.
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseInMemoryDatabase("PatientDb"));

builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseInMemoryDatabase("AppointmentDb"));

// Use PostgreSQL for Prescription outbox so background host reads real messages from Postgres.
var pgConn = builder.Configuration["ConnectionStrings:Postgres"]
             ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
             ?? "Host=localhost;Port=5432;Database=clinic_db;Username=clinic_user;Password=clinic_password";

if (!string.IsNullOrWhiteSpace(pgConn))
{
    builder.Services.AddDbContext<PrescriptionDbContext>(options =>
        options.UseNpgsql(pgConn));
}
else
{
    builder.Services.AddDbContext<PrescriptionDbContext>(options =>
        options.UseInMemoryDatabase("PrescriptionDb"));
}

// 2. Register Outbox Readers (one per module)
builder.Services.AddTransient<IOutboxReader, EntityFrameworkOutboxReader<PatientDbContext>>();
builder.Services.AddTransient<IOutboxReader, EntityFrameworkOutboxReader<AppointmentDbContext>>();
builder.Services.AddTransient<IOutboxReader, EntityFrameworkOutboxReader<PrescriptionDbContext>>();

// 3. Register Outbox Publisher (RabbitMQ)
builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
{
    HostName = "localhost",
    DispatchConsumersAsync = true
});
builder.Services.AddTransient<IOutboxPublisher, RabbitMQOutboxPublisher>();

// 4. Configure Quartz
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("OutboxPublishingJob");
    q.AddJob<OutboxPublishingJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("OutboxPublishingTrigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();

// Ensure the Prescription Postgres schema is created so the outbox table exists.
using (var scope = host.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<PrescriptionDbContext>();
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Log and continue - if DB not available yet, host will still run and job will retry.
        Console.WriteLine($"Warning: failed to ensure database created: {ex.Message}");
    }
}

host.Run();
