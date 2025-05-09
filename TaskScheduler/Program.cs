using TaskScheduler;
using TaskScheduler.Interfaces;
using TaskScheduler.Models;
using TaskScheduler.Services;

await Task.Delay(TimeSpan.FromSeconds(5));

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient("ClientApi", client =>
{
    var clientBaseUrl = builder.Configuration["ClientBaseUrl"];
    client.BaseAddress = new Uri(clientBaseUrl);
});

builder.Services.Configure<List<ScheduledTaskConfig>>(builder.Configuration.GetSection("ScheduledJobs"));
builder.Services.AddSingleton<ITimeTriggeredJob, CleanUpOldLogsJob>();
builder.Services.AddSingleton<ITimeTriggeredJob, CheckSystemHealthJob>();
builder.Services.AddSingleton<ITimeTriggeredJob, ToggleAirJob>();
builder.Services.AddSingleton<ITimeTriggeredJob, ToggleBoatJob>();
builder.Services.AddSingleton<ITimeTriggeredJob, ToggleSunJob>();
builder.Services.AddSingleton<ITimeTriggeredJob, ToggleTruckJob>();
builder.Services.AddScoped<IHttpClientFactoryService, HttpClientFactoryService>();

builder.Services.AddTransient<IJobService, JobService>();
builder.Services.AddHostedService<TaskSchedulerBackgroundService>();
var host = builder.Build();
host.Run();
