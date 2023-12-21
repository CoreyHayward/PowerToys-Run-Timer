namespace Community.PowerToys.Run.Plugin.Timers;

/// <summary>
/// Extension of <see cref="System.Timers.Timer"/> which 
/// adds the ability to get the time left on the timer.
/// </summary>
public class TimerPlus : System.Timers.Timer
{
    private DateTime _dueTime;

    public TimerPlus(TimeSpan timeSpan) : base(timeSpan) => Elapsed += ElapsedAction;

    protected new void Dispose()
    {
        Elapsed -= ElapsedAction;
        base.Dispose();
    }

    public TimeSpan TimeLeft => TimeSpan.FromMilliseconds((_dueTime - DateTime.Now).TotalMilliseconds);

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
