using TaskScheduler.Interfaces;

namespace TaskScheduler;

public class CheckSystemHealthJob : ITimeTriggeredJob
{
    public string Name => "CheckSystemHealth";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }


    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs CheckSystemHealth at {DateTime.Now}, next {NextOccurrence}.");
        return Task.CompletedTask;
    }
}
