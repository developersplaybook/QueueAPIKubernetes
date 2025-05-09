using TaskScheduler.Interfaces;
using Microsoft.Extensions.Options;
using NCrontab;

namespace TaskScheduler;

public class TaskSchedulerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ScheduledTaskConfig> _jobConfigs;
    private readonly Dictionary<string, CrontabSchedule> _schedules = new();
    private readonly Dictionary<string, DateTime> _nextRuns = new();

    public TaskSchedulerBackgroundService(IOptions<List<ScheduledTaskConfig>> options, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _jobConfigs = options.Value;

        foreach (var job in _jobConfigs.Where(j => j.Enabled))
        {
            var schedule = CrontabSchedule.Parse(job.Cron);
            _schedules[job.Name] = schedule;
            _nextRuns[job.Name] = schedule.GetNextOccurrence(ThisMinute());
        }
    }

    private DateTime ThisMinute()
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    }


    async Task DelayUntilNextWholeMinuteAsync(CancellationToken cancellationToken)
    {
        var delay = ThisMinute().AddMinutes(1) - DateTime.Now;
        await Task.Delay(delay, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope _scope = ExecuteAirJobOnceAtStart(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = ThisMinute();

            foreach (var jobConfig in _jobConfigs.Where(j => j.Enabled))
            {
                if (_nextRuns[jobConfig.Name] <= now)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var job = scope.ServiceProvider
                        .GetServices<ITimeTriggeredJob>()
                        .FirstOrDefault(j => j.Name == jobConfig.Name);

                    if (job != null)
                    {
                        var _schedule = _schedules[jobConfig.Name];
                        var next = _schedule.GetNextOccurrence(_nextRuns[jobConfig.Name]);
                        _nextRuns[jobConfig.Name] = next;
                        job.NextOccurrence = _nextRuns[jobConfig.Name].ToString("HH:mm:ss");
                        _ = Task.Run(() => job.ExecuteAsync(stoppingToken), stoppingToken);
                    }
                }
            }


            await DelayUntilNextWholeMinuteAsync(stoppingToken);
        }

        IServiceScope ExecuteAirJobOnceAtStart(CancellationToken stoppingToken)
        {
            var _scope = _serviceProvider.CreateScope();
            var _job = _scope.ServiceProvider
                .GetRequiredService<IEnumerable<ITimeTriggeredJob>>()
                .FirstOrDefault(j => j.Name == "ToggleAirJob");

            if (_job != null)
            {
                _job.NextOccurrence = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss");
                _ = Task.Run(() => _job.ExecuteAsync(stoppingToken), stoppingToken);
            }

            return _scope;
        }
    }
}