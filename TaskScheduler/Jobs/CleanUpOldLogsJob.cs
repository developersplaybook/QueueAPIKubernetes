using TaskScheduler.Interfaces;

namespace TaskScheduler;

public class CleanUpOldLogsJob : ITimeTriggeredJob
{
    public string Name => "CleanUpOldLogs";
    private string _nextOccurrence = string.Empty;
    public string NextOccurrence
    {
        get => _nextOccurrence;
        set => _nextOccurrence = value;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Runs CleanUpOldLogs at {DateTime.Now}, next {NextOccurrence}.");
        return Task.CompletedTask;
    }
}
