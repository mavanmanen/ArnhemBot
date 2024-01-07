using Discord;
using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IDiscordBotClient
{
    public Task StartAsync(string token);
}

public class DiscordBotClient(
    DiscordSocketClient client,
    ICommandHandler commandHandler,
    ILoggingService loggingService,
    IRandomActivityService randomActivityService) : IDiscordBotClient
{
    public async Task StartAsync(string token)
    {
        client.Log += loggingService.LogAsync;
        client.SlashCommandExecuted += commandHandler.HandleCommandAsync;
        client.GuildAvailable += commandHandler.RegisterCommandsAsync;
        
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
        randomActivityService.Start();
    }
}