using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Common.Helpers;

public class StopwatchHelper
{
    private readonly ILogger<StopwatchHelper> _logger;

    public StopwatchHelper(ILogger<StopwatchHelper> logger)
    {
        _logger = logger;
    }

    public void LogActionWorkTime(string actionTitle,LogLevel logLevel, Action action)
    {
        var workTime = GetActionWorkTime(action);

        _logger.Log(logLevel, $"{actionTitle} Work time: {workTime}");
    }
    
    public TimeSpan GetActionWorkTime(Action action)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        action.Invoke(); 

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}