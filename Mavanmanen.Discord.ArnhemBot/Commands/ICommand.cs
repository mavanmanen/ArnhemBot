using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Commands;

public interface ICommand
{
    public Task<bool> ExecuteAsync(SocketSlashCommand command);
}