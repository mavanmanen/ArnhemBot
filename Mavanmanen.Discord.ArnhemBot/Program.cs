using System.Reflection;
using System.Text.Json;
using Mavanmanen.Discord.ArnhemBot.Commands;
using Mavanmanen.Discord.ArnhemBot.Services;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using RestSharp.Serializers.Json;

var services = new ServiceCollection();

services.AddSingleton<ILoggingService, LoggingService>();
services.AddSingleton<IDeferredCommandHandler, DeferredCommandHandler>();
services.AddSingleton<ICommandHandler, CommandHandler>();
services.AddScoped<IMementoApiClient, MementoApiClient>();
services.AddSingleton<IDiscordBotClient, DiscordBotClient>();

var commandTypes = Assembly.GetExecutingAssembly().DefinedTypes
    .Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

foreach (var commandType in commandTypes)
{
    services.AddScoped(commandType);
    services.AddScoped(typeof(ICommand), commandType);
}

var serviceProvider = services.BuildServiceProvider();

var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (botToken is null)
{
    Console.Write("Enter the bot token: ");
    botToken = Console.ReadLine() ?? throw new Exception("Bot token must be present");
}

var client = serviceProvider.GetRequiredService<IDiscordBotClient>();
await client.StartAsync(botToken);
await Task.Delay(Timeout.Infinite);