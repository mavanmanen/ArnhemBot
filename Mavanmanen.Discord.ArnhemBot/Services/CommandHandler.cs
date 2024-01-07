using System.Reflection;
using Discord;
using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Attributes;
using Mavanmanen.Discord.ArnhemBot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ICommandHandler
{
    public Task HandleCommandAsync(SocketSlashCommand command);
    public Task RegisterCommandsAsync(SocketGuild guild);
}

public class CommandHandler(
    IServiceProvider serviceProvider,
    ICommandDiscoverer commandDiscoverer,
    ILoggingService loggingService,
    IDeferredCommandHandler deferredCommandHandler)
    : ICommandHandler
{
    public async Task RegisterCommandsAsync(SocketGuild guild)
    {
        ApplicationCommandProperties[] builtCommands = await Task.WhenAll(commandDiscoverer.CommandTypes.Select(BuildCommandAsync));
        await guild.BulkOverwriteApplicationCommandAsync(builtCommands);
    }

    private async Task<SlashCommandProperties> BuildCommandAsync(Type commandType)
    {
        var commandAttribute = commandType.GetCustomAttribute<CommandAttribute>()!;
        var commandOptionAttributes = commandType.GetCustomAttributes<CommandOptionAttribute>().ToArray();
        
        var builder = new SlashCommandBuilder()
            .WithName(commandAttribute.Name)
            .WithDescription(commandAttribute.Description);

        foreach (var option in commandOptionAttributes)
        {
            builder.AddOption(option.Name, option.Type, option.Description, option.IsRequired);
        }
        
        await loggingService.LogInfoAsync(nameof(CommandHandler), $"Registering command: {commandAttribute.Name}");

        return builder.Build();
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        await loggingService.LogInfoAsync(nameof(CommandHandler), $"Executing command: {SerializeCommand(command)}");

        deferredCommandHandler.DeferCommand(async () =>
        {
            await command.DeferAsync();
            await using var scope = serviceProvider.CreateAsyncScope();
            var commandInstance = scope.ServiceProvider.GetRequiredKeyedService<ICommand>(command.Data.Name);
            var result = await commandInstance.ExecuteAsync(command);
            if (!result)
            {
                await command.DeleteOriginalResponseAsync();
                await command.FollowupAsync("Something went wrong..", ephemeral: true);
                await loggingService.LogErrorAsync(nameof(CommandHandler), $"Failed to handle command: {SerializeCommand(command)}");
            }
        });
    }

    private static string SerializeCommand(SocketSlashCommand command) =>
        $"{command.Data.Name}: {string.Join(" ", command.Data.Options.Select(o => $"[{o.Name} ({o.Type:G})]: {o.Value}"))}";
}