using Discord;
using Discord.WebSocket;
using Mavanmanen.Discord.ArnhemBot.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<ILoggingService, LoggingService>();
services.AddSingleton<ICommandDiscoverer>(new CommandDiscoverer(services));
services.AddSingleton<IDeferredCommandHandler, DeferredCommandHandler>();
services.AddSingleton<ICommandHandler, CommandHandler>();
services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
}));
services.AddSingleton<IDiscordBotClient, DiscordBotClient>();
services.AddSingleton<IRandomActivityService, RandomActivityService>();

var serviceProvider = services.BuildServiceProvider();

var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN")
               ?? throw new Exception("BOT_TOKEN environment variable must be present");

var client = serviceProvider.GetRequiredService<IDiscordBotClient>();
await client.StartAsync(botToken);
await Task.Delay(Timeout.Infinite);