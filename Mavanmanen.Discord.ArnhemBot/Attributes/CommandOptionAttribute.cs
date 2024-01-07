using Discord;

namespace Mavanmanen.Discord.ArnhemBot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandOptionAttribute(
    string name,
    ApplicationCommandOptionType type,
    string description,
    bool isRequired = false) : Attribute
{
    public string Name { get; } = name;
    public ApplicationCommandOptionType Type { get; } = type;
    public string Description { get; } = description;
    public bool IsRequired { get; } = isRequired;
}