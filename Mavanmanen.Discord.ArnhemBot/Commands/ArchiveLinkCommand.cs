using Discord;
using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Services;

namespace Mavanmanen.Discord.ArnhemBot.Commands;

public class ArchiveLinkCommand(IMementoApiClient mementoApiClient) : ICommand
{
    public string Name => "archive-link";

    public ApplicationCommandProperties Build() =>
        new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription("Get the archive.ph link for another link.")
            .AddOption("link", ApplicationCommandOptionType.String, "The link to search for.", isRequired: true)
            .Build();

    public async Task ExecuteAsync(SocketSlashCommand command)
    {
        var originalLink = ((string)command.Data.Options.Single().Value).Trim();

        var result = await mementoApiClient.GetResultsAsync(originalLink);
        if (result is null)
        {
            await command.ModifyOriginalResponseAsync(properties => properties.Content = "No results.");
            return;
        }
        
        var archivedLink = result.MementoInfo.FirstOrDefault(r => r.ArchiveId == "archive.is");
        if (archivedLink is null)
        {
            await command.ModifyOriginalResponseAsync(properties => properties.Content = "No results.");
            return;
        }

        await command.ModifyOriginalResponseAsync(properties => properties.Content = archivedLink.TimegateUri.ToString());
    }
}