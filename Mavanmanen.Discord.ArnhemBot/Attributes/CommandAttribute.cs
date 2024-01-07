﻿namespace Mavanmanen.Discord.ArnhemBot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
}