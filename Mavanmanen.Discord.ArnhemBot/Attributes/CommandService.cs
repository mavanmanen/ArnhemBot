using Microsoft.Extensions.DependencyInjection;

namespace Mavanmanen.Discord.ArnhemBot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandServiceAttribute(ServiceLifetime lifetime, Type serviceType, Type implementationType) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public Type ServiceType { get; } = serviceType;
    public Type ImplementationType { get; } = implementationType;
}