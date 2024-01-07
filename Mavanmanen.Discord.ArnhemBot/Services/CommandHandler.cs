using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ICommandHandler
{
    public Task HandleCommandAsync(SocketSlashCommand command);
    public Task RegisterCommandsAsync(DiscordSocketClient client);
}

public class CommandHandler(
    IServiceProvider serviceProvider,
    ILoggingService loggingService,
    IDeferredCommandHandler deferredCommandHandler)
    : ICommandHandler
{
    private readonly Dictionary<string, Type> _commandTypes = new();

    public async Task RegisterCommandsAsync(DiscordSocketClient client)
    {
        var commands = serviceProvider.GetServices<ICommand>().ToArray();
        foreach (var command in commands)
        {
            _commandTypes.Add(command.Name, command.GetType());
            await loggingService.LogInfoAsync(nameof(CommandHandler), $"Registering command: {command.Name}");
        }

        var builtCommands = commands.Select(c => c.Build()).ToArray();
        
        await client.BulkOverwriteGlobalApplicationCommandsAsync(builtCommands);
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        await command.DeferAsync();

        await loggingService.LogInfoAsync(nameof(CommandHandler), $"Executing command: {SerializeCommand(command)}");
        var commandType = _commandTypes[command.Data.Name];
        
        deferredCommandHandler.DeferCommand(async () =>
        {
            using var scope = serviceProvider.CreateScope();
            var commandInstance = (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
            await commandInstance.ExecuteAsync(command);
        });
    }

    private static string SerializeCommand(SocketSlashCommand command) =>
        $"{command.Data.Name}: {string.Join(" ", command.Data.Options.Select(o => $"[{o.Name} ({o.Type:G})]: {o.Value}"))}";
}