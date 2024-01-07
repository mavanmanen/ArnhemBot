using Discord;
using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IDiscordBotClient
{
    public Task StartAsync(string token);
}

public class DiscordBotClient : IDiscordBotClient
{
    private readonly ILoggingService _loggingService;
    private readonly DiscordSocketClient _client = new();
    
    public DiscordBotClient(ICommandHandler commandHandler, ILoggingService loggingService)
    {
        _loggingService = loggingService;
        _client.Log += _loggingService.LogAsync;
        _client.Ready += () => commandHandler.RegisterCommandsAsync(_client);
        _client.SlashCommandExecuted += commandHandler.HandleCommandAsync;
    }

    public async Task StartAsync(string token)
    {
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }
}