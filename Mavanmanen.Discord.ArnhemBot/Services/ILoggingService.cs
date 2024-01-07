using Discord;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ILoggingService
{
    public Task LogAsync(LogMessage message);
    
    public Task LogAsync(LogSeverity severity, string source, string message) =>
        LogAsync(new LogMessage(severity, source, message));

    public Task LogInfoAsync(string source, string message) =>
        LogAsync(LogSeverity.Info, source, message);

    public Task LogErrorAsync(string source, string message) =>
        LogAsync(LogSeverity.Error, source, message);
}