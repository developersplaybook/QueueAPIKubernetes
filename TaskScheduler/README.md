# TaskScheduler (.NET 8)

A lightweight replacement for Hangfire/Quartz based on BackgroundService and CRON expressions in appsettings.json.

## Functionality

- Reads CRON configurations from appsettings.json
- Checks every minute if a job should run
- No external dependencies like Hangfire or Quartz.NET

## Example Jobs
- CleanUpOldLogs — runs daily at 02:00
- CheckSystemHealth — runs every 5 minutes
- ToggleAirJob — runs every minute
- ToggleBoatJob — runs every second minute
- ToggleSunJob — runs every third minute
- ToggleTruckJob — runs every fourth minute

## Start

```bash
dotnet restore
dotnet run
```

## Add Your Own Jobs
- Create a class that implements ITimeTriggeredJob
- Register it in Program.cs
- Add a CRON expression in appsettings.json
