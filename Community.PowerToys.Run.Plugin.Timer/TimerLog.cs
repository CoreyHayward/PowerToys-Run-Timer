
using Humanizer;
using System.Timers;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.Timers;

public class TimerLog
{
    private List<TimerLogItem> _timerLogItems = new List<TimerLogItem>();
    public int Count => _timerLogItems.Count;
    public TimerLogItem this[int index] => _timerLogItems[index];
    private readonly PluginInitContext _pluginContext;

    public TimerLog(PluginInitContext pluginContext)
    {
        ArgumentNullException.ThrowIfNull(pluginContext);
        _pluginContext = pluginContext;
    }

    public void AddTimer(TimeSpan timeSpan, string title)
    {
        _timerLogItems.Add(new TimerLogItem(timeSpan, title, _pluginContext));
    }

    public void AddTimer(string title)
    {
        _timerLogItems.Add(new TimerLogItem(title, _pluginContext));
    }

    public void RemoveTimerLog(TimerLogItem timer)
    {
        timer.RemoveTimer();
        _timerLogItems.Remove(timer);
    }

    public void ShowNotification(string title, string message)
    {
        _pluginContext!.API.ShowNotification(title, message);
    }
}

public class TimerLogItem
{
    TimerPlus? timer;
    private bool _IsCountdownTimer = true;
    private readonly PluginInitContext _pluginContext;
    private DateTime? _endTime;
    private DateTime? _startTime;
    public readonly string Title;
    public bool IsRunning => timer != null;
    public bool IsPaused { get; private set; }
    public bool HasFinished { get; set; } = false;
    private TimeSpan _TimeElapsed { get; set; } = TimeSpan.Zero;
    public TimeSpan TimeElapsed
    {
        get
        {
            if (!_IsCountdownTimer)
            {
                if (timer != null)
                {
                    return timer.TimeElapsed + _TimeElapsed;
                }
                return _TimeElapsed;
            }
            if (timer != null)
            {
                return timer.TimeElapsed;
            }
            return _TimeElapsed;
        }
    }
    private TimeSpan _TimeLeft { get; set; } = TimeSpan.Zero;
    public TimeSpan TimeLeft
    {
        get
        {
            if (timer != null)
            {
                return timer.TimeLeft;
            }
            return _TimeLeft;

        }
    }
    private double _Interval { get; set; } = 0;
    public double Interval
    {
        get
        {
            if (timer != null)
            {
                return timer.Interval;
            }
            return _Interval;
        }
    }

    public TimerLogItem(TimeSpan timeSpan, string title, PluginInitContext pluginContext)
    {
        _pluginContext = pluginContext;
        timer = new TimerPlus(timeSpan, title);
        Title = title;
        _startTime = DateTime.Now;
        timer.AutoReset = false;
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    public TimerLogItem(string title, PluginInitContext pluginContext)
    {
        _pluginContext = pluginContext;
        Title = title;
        timer = new TimerPlus(title);
        timer.Start();
        _startTime = DateTime.Now;
        _IsCountdownTimer = false;
    }

    public string GetTitle()
    {
        if (_IsCountdownTimer)
        {
            var timeInterval = TimeSpan.FromMilliseconds(Interval);
            if (String.IsNullOrWhiteSpace(Title))
            {
                return $"{Humanize(timeInterval)} timer";
            }
            return $"{Title} ({Humanize(timeInterval)} timer)";
        }
        if (String.IsNullOrWhiteSpace(Title))
        {
            return "Timer";
        }
        return $"{Title} timer";
    }

    public string GetSubTitle()
    {
        if (_IsCountdownTimer)
        {
            return $"{Humanize(TimeLeft)} left";
        }
        return $"{Humanize(TimeElapsed)} elapsed";
    }

    public void PauseTimer()
    {
        if (timer != null)
        {
            _Interval = timer.Interval;
            if (_IsCountdownTimer)
            {
                _TimeLeft = timer.TimeLeft;
            }
            else
            {
                _TimeElapsed = _TimeElapsed + timer.TimeElapsed;
            }
            timer.Stop();
            timer.Dispose();
            timer = null;
            _endTime = DateTime.Now;
            this.IsPaused = true;
        }
    }

    public void ResumeTimer()
    {
        if (_IsCountdownTimer)
        {
            timer = new TimerPlus(_TimeLeft, Title);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            _startTime = DateTime.Now;
        }
        else
        {
            timer = new TimerPlus(Title);
        }
        timer.Start();
        _endTime = null;
        this.IsPaused = false;
    }

    public void RemoveTimer()
    {
        if (timer != null)
        {
            timer.Stop();
            timer.Dispose();
        }
        timer = null;
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        var timeSpan = TimeSpan.FromMilliseconds(Interval);
        timer!.Stop();
        timer!.Stop();
        timer!.Dispose();

        _Interval = timer.Interval;
        _TimeLeft = timer.TimeLeft;
        HasFinished = true;

        if (string.IsNullOrWhiteSpace(timer.Title))
        {
            _pluginContext!.API.ShowNotification($"{Humanize(timeSpan).Singularize(false)} timer elapsed.");
        }
        else
        {
            _pluginContext!.API.ShowNotification(timer.Title, $"{Humanize(timeSpan).Singularize(false)} timer elapsed.");
        }

        RemoveTimer();
    }
    private static string Humanize(TimeSpan ts)
    => ts.Humanize(precision: 3, minUnit: Humanizer.Localisation.TimeUnit.Second);
}
