using Microsoft.Extensions.Logging;

namespace Common.Extensions;

public static class LoggerExtensions
{
    public static void LogProgress(this ILogger logger, long current, long total, string? prefix = null, bool logOnlyProcent = true)
    {
        prefix = prefix ?? "unknown";

        if (current >= total)
            logger.LogInformation( $"{prefix}: Finished.");

        var procentOfProgress = (long)(((double)current / total) * 100);
        
        if(logOnlyProcent)
            logger.LogInformation($"{prefix}: procent of progress is {procentOfProgress}%.");
        else
            logger.LogInformation($"{prefix}: procent of progress is {procentOfProgress}%(Current:{current} Total:{total}).");
    }
}