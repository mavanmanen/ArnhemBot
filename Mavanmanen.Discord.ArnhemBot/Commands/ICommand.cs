using Discord;
using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Commands;

public interface ICommand
{
    public string Name { get; }
    public ApplicationCommandProperties Build();
    public Task ExecuteAsync(SocketSlashCommand command);
}