using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ICommandHandler
{
    public Task HandleCommandAsync(SocketSlashCommand command);
    public Task RegisterCommandsAsync(DiscordSocketClient client);
}

public class CommandHandler : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggingService _loggingService;
    private readonly Dictionary<string, Type> _commandTypes = new();
    private readonly List<Task> _runningCommands = new();
    private readonly Timer _timer;
    
    public CommandHandler(IServiceProvider serviceProvider, ILoggingService loggingService)
    {
        _serviceProvider = serviceProvider;
        _loggingService = loggingService;
        var period = TimeSpan.FromSeconds(5);
        _timer = new Timer(CheckRunningCommands, null, period, period);
    }
    
    private void CheckRunningCommands(object? _)
    {
        _runningCommands.RemoveAll(c => c.IsCompleted);
    }

    public async Task RegisterCommandsAsync(DiscordSocketClient client)
    {
        var commands = _serviceProvider.GetServices<ICommand>().ToArray();
        foreach (var command in commands)
        {
            _commandTypes.Add(command.Name, command.GetType());
            await _loggingService.LogInfoAsync(nameof(CommandHandler), $"Registering command: {command.Name}");
        }

        var builtCommands = commands.Select(c => c.Build()).ToArray();
        
        await client.BulkOverwriteGlobalApplicationCommandsAsync(builtCommands);
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        await _loggingService.LogInfoAsync(nameof(CommandHandler), $"Executing command: {SerializeCommand(command)}");
        
        await command.DeferAsync();
        var commandType = _commandTypes[command.Data.Name];
        var commandTask = Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var commandInstance = (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
            await commandInstance.ExecuteAsync(command);
        });
        
        _runningCommands.Add(commandTask);
    }

    private static string SerializeCommand(SocketSlashCommand command) =>
        $"{command.Data.Name}: {string.Join(" ", command.Data.Options.Select(o => $"[{o.Name} ({o.Type:G})]: {o.Value}"))}";
}