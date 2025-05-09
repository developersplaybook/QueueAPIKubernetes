using TaskScheduler.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler;

public class ToggleAirJob : ITimeTriggeredJob
{
    private readonly IJobService _jobService;

    public ToggleAirJob(IJobService jobService)
    {
        _jobService = jobService;
    }
    public string Name => "ToggleAirJob";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs {Name} at {DateTime.Now}, next {NextOccurrence}.");
        _jobService.TriggerJobToggle(JobNames.AirJob);
        return Task.CompletedTask;
    }
}
