using System.Reflection;
using Mavanmanen.Discord.ArnhemBot.Attributes;
using Mavanmanen.Discord.ArnhemBot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface ICommandDiscoverer
{
    public TypeInfo[] CommandTypes { get; }
}

public class CommandDiscoverer : ICommandDiscoverer
{
    public TypeInfo[] CommandTypes { get; private set; } = null!;

    public CommandDiscoverer(IServiceCollection services)
    {
        DiscoverAndRegisterCommands(services);
    }

    public void DiscoverAndRegisterCommands(IServiceCollection services)
    {
        CommandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.DefinedTypes
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand))))
            .ToArray();

        foreach (var commandType in CommandTypes)
        {
            var name = commandType.GetCustomAttribute<CommandAttribute>()!.Name;
            services.AddKeyedScoped(typeof(ICommand), name, commandType);

            var commandServices = commandType.GetCustomAttributes<CommandServiceAttribute>()
                .Select(c => new ServiceDescriptor(c.ServiceType, c.ImplementationType, c.Lifetime));

            services.Add(commandServices);
        }
    }
}