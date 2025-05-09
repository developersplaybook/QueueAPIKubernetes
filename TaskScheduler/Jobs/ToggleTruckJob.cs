using TaskScheduler.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler;

public class ToggleTruckJob : ITimeTriggeredJob
{
    private readonly IJobService _jobService;

    public ToggleTruckJob(IJobService jobService)
    {
        _jobService = jobService;
    }
    public string Name => "ToggleTruckJob";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs {Name} at {DateTime.Now}, next {NextOccurrence}.");
        _jobService.TriggerJobToggle(JobNames.TruckJob);
        return Task.CompletedTask;
    }
}
