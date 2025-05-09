using TaskScheduler.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler;

public class ToggleBoatJob : ITimeTriggeredJob
{
    private readonly IJobService _jobService;

    public ToggleBoatJob(IJobService jobService)
    {
        _jobService = jobService;
    }
    public string Name => "ToggleBoatJob";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs {Name} at {DateTime.Now}, next {NextOccurrence}.");
        _jobService.TriggerJobToggle(JobNames.BoatJob);
        return Task.CompletedTask;
    }
}
