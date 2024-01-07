using Discord;
using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IDiscordBotClient
{
    public Task StartAsync(string token);
}

public class DiscordBotClient(ICommandHandler commandHandler, ILoggingService loggingService) : IDiscordBotClient
{
    private readonly DiscordSocketClient _client = new();

    public async Task StartAsync(string token)
    {
        _client.Log += loggingService.LogAsync;
        _client.Ready += () => commandHandler.RegisterCommandsAsync(_client);
        _client.SlashCommandExecuted += commandHandler.HandleCommandAsync;
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }
}