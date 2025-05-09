namespace TaskScheduler;

public class ScheduledTaskConfig
{
    public string Name { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
    public bool Enabled {  get; set; } = true;
    public string NextOccurrence {  get; set; } = string.Empty;
}
