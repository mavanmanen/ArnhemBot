using Discord;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public class LoggingService : ILoggingService
{
    public Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}