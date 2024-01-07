namespace Mavanmanen.Discord.ArnhemBot.Services;

public interface IDeferredCommandHandler
{
    public void DeferCommand(Func<Task> command);
}

public class DeferredCommandHandler : IDeferredCommandHandler
{
    private readonly List<Task> _deferredCommands = [];
    private readonly Timer _timer;

    public DeferredCommandHandler()
    {
        _timer = new Timer(CheckRunningCommands, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }
    
    private void CheckRunningCommands(object? _) => _deferredCommands.RemoveAll(c => c.IsCompleted);

    public void DeferCommand(Func<Task> command) => _deferredCommands.Add(Task.Run(command));
}