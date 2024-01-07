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

var secret = File.ReadAllText("./bottoken");

var client = serviceProvider.GetRequiredService<IDiscordBotClient>();
await client.StartAsync(secret);
await Task.Delay(Timeout.Infinite);