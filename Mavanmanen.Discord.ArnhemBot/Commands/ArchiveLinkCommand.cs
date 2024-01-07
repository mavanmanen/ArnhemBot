using Discord;
using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Attributes;
using Mavanmanen.Discord.ArnhemBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mavanmanen.Discord.ArnhemBot.Commands;

[Command("archive-link", "Get the archive.ph link for another link")]
[CommandOption("link", ApplicationCommandOptionType.String, "The link to search for", isRequired: true)]
[CommandService(ServiceLifetime.Scoped, typeof(IMementoApiClient), typeof(MementoApiClient))]
public class ArchiveLinkCommand(IMementoApiClient mementoApiClient, ILoggingService loggingService) : ICommand
{
    public async Task<bool> ExecuteAsync(SocketSlashCommand command)
    {
        var originalLink = ((string)command.Data.Options.Single().Value).Trim();
        var originalLinkEmbed = await command.ModifyOriginalResponseAsync(properties => properties.Content = $"Getting archive link for: {originalLink}");

        var archivedLink = await mementoApiClient.GetResultsAsync(originalLink);
        if (archivedLink is null)
        {
            await loggingService.LogErrorAsync(nameof(ArchiveLinkCommand), $"Failed");
            return false;
        }

        while (originalLinkEmbed.Embeds.Count == 0)
        {
            await Task.Delay(100);
        }
        await originalLinkEmbed.ModifyAsync(properties =>
        {
            properties.Content = archivedLink;

            var embed = originalLinkEmbed.Embeds.Single();

            properties.Embed = new EmbedBuilder()
                .WithTitle(embed.Title)
                .WithDescription(embed.Description)
                .WithAuthor(embed.Author?.Name)
                .WithUrl(archivedLink)
                .WithImageUrl(embed.Image?.Url ?? embed.Thumbnail?.Url)
                .Build();
        });
        
        return true;
    }
}