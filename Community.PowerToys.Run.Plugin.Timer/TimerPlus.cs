namespace Community.PowerToys.Run.Plugin.Timers;

/// <summary>
/// Extension of <see cref="System.Timers.Timer"/> which 
/// adds the ability to get the time left on the timer.
/// </summary>
public class TimerPlus : System.Timers.Timer
{
    private DateTime _dueTime;
    public readonly string Title;
    public TimeSpan TimeLeft => TimeSpan.FromMilliseconds((_dueTime - DateTime.Now).TotalMilliseconds);

    public TimerPlus(TimeSpan timeSpan, string title = "") : base(timeSpan)
    {
        Elapsed += ElapsedAction;
        Title = title;
    }

    protected new void Dispose()
    {
        Elapsed -= ElapsedAction;
        base.Dispose();
    }

    public new void Start()
    {
        _dueTime = DateTime.Now.AddMilliseconds(Interval);
        base.Start();
    }

    private void ElapsedAction(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (AutoReset)
        {
            _dueTime = DateTime.Now.AddMilliseconds(Interval);
        }
    }
}
