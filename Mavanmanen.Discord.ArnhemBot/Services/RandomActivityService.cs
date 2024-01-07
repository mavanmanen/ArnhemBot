using Discord;
using Discord.WebSocket;

namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IRandomActivityService
{
    public void Start();
}

public class RandomActivityService(DiscordSocketClient client) : IRandomActivityService
{
    private Timer? _timer;
    private int _position;

    private string[] _activities = [
        "Arnhem > Nijmegen",
        "Met Marcouch de straat opruimen",
        "Zuipen op de Korenmarkt",
        "Zwemmen in de Rijn"
    ];

    public void Start()
    {
        _timer = new Timer(ChangeActivityAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private async void ChangeActivityAsync(object? _)
    {
        await client.SetActivityAsync(new CustomStatusGame(_activities[_position]));
        if (++_position == _activities.Length)
        {
            _position = 0;
        }
    }
}