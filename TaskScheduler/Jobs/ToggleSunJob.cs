using TaskScheduler.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler;

public class ToggleSunJob : ITimeTriggeredJob
{
    private readonly IJobService _jobService;

    public ToggleSunJob(IJobService jobService)
    {
        _jobService = jobService;
    }
    public string Name => "ToggleSunJob";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs {Name} at {DateTime.Now}, next {NextOccurrence}.");
        _jobService.TriggerJobToggle(JobNames.SunJob);
        return Task.CompletedTask;
    }
}
