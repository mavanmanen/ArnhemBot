using Discord;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ILoggingService
{
    public Task LogAsync(LogMessage message);
    
    public Task LogAsync(LogSeverity severity, string source, string message) =>
        LogAsync(new LogMessage(severity, source, message));

    public Task LogInfoAsync(string source, string message) =>
        LogAsync(LogSeverity.Info, source, message);
}

public class LoggingService : ILoggingService
{
    public Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}