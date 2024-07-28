using Humanizer;
using ManagedCommon;
using System.Timers;
using System.Windows.Input;
using TimeSpanParserUtil;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.Timers;
public class TimerResultService
{
    private readonly List<TimerPlus> _timers = [];
    private readonly TimerLog _timerLog;
    private readonly PluginInitContext _pluginContext;
    private string _iconPath = "Images/Timer.light.png";

    public TimerResultService(PluginInitContext pluginContext)
    {
        ArgumentNullException.ThrowIfNull(pluginContext);
        _pluginContext = pluginContext;
        _pluginContext.API.ThemeChanged += (_, theme) => UpdateTheme(theme);
        UpdateTheme(_pluginContext.API.GetCurrentTheme());
        _timerLog = new TimerLog(_pluginContext);
    }

    private void UpdateTheme(Theme theme)
    {
        if (theme == Theme.Light || theme == Theme.HighContrastWhite)
        {
            _iconPath = "Images/Timer.light.png";
        }
        else
        {
            _iconPath = "Images/Timer.dark.png";
        }
    }

    public List<Result> GetQueryResult(Query query, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(query.Search))
        {
            return GetRunningTimersResults(query.Search);
        }

        return GetCreateTimerResult(query.Search, settings);
    }

    private List<Result> GetRunningTimersResults(string search)
    {
        var timerResults = new List<Result>();
        for (var i = 0; i < _timerLog.Count; i++)
        {
            var timer = _timerLog[i];
            timerResults.Add(new Result()
            {
                QueryTextDisplay = search,
                Title = timer.GetTitle(),
                SubTitle = timer.GetSubTitle(),
                IcoPath = _iconPath,
                Action = _ => true,
                ContextData = timer,
            });
        }

        return timerResults;
    }

    public List<Result> GetCreateTimerResult(string query, Settings settings)
    {
        query = query.Trim();

        if (!TimeSpanParser.TryParse(query, settings.TimeSpanParserOptions, out var timeSpan) || timeSpan <= TimeSpan.Zero)
        {
            // start a count "UP" timer
            var resultUp = new Result()
            {
                QueryTextDisplay = query,
                Title = "Start timer",
                SubTitle = $"Starts an ongoing timer.",
                IcoPath = _iconPath,
                Action = _ =>
                {
                    _timerLog.AddTimer(query);
                    return true;
                }
            };

            return [resultUp];
        }

        var timerTitle = ParseTimerTitleFromQuery(query, timeSpan);
        var result = new Result()
        {
            QueryTextDisplay = query,
            Title = "Start timer",
            SubTitle = $"Starts a {Humanize(timeSpan).Singularize(false)} timer.",
            IcoPath = _iconPath,
            Action = _ =>
            {
                _timerLog.AddTimer(timeSpan, timerTitle);
                return true;
            }
        };

        return [result];
    }

    public List<ContextMenuResult> GetContextMenuResults(Result selectedResult)
    {
        var timer = selectedResult.ContextData as TimerLogItem;
        if (timer is null) return [];
        List<ContextMenuResult> contexts = new List<ContextMenuResult>();
        if (timer.IsRunning)
        {
            var pauseTimer = new ContextMenuResult()
            {
                Title = "Pause Timer (Ctrl+Space)",
                Glyph = "\xE769",
                FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                AcceleratorKey = Key.Space,
                AcceleratorModifiers = ModifierKeys.Control,
                Action = _ =>
                {
                    timer.PauseTimer();
                    return true;
                },
            };
            contexts.Add(pauseTimer);
        }
        else if (timer.IsPaused)
        {
            var resumeTimer = new ContextMenuResult()
            {
                Title = "Resume Timer (Ctrl+Space)",
                Glyph = "\xE768",
                FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                AcceleratorKey = Key.Space,
                AcceleratorModifiers = ModifierKeys.Control,
                Action = _ =>
                {
                    timer.ResumeTimer();
                    return true;
                },
            };
            contexts.Add(resumeTimer);
        }

        var deleteTimer = new ContextMenuResult()
        {
            Title = "Delete Timer (Ctrl+Enter)",
            Glyph = "\xE74D",
            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            AcceleratorKey = Key.Enter,
            AcceleratorModifiers = ModifierKeys.Control,
            Action = _ =>
            {
                _timerLog.RemoveTimerLog(timer);
                _pluginContext.API.RemoveUserSelectedItem(selectedResult);
                return true;
            },
        };
        contexts.Add(deleteTimer);

        return contexts;
    }

    private static string ParseTimerTitleFromQuery(string query, TimeSpan timeSpan)
    {
        var words = query!.Split(' ');
        for (var i = words.Length - 1; i >= 0; i--)
        {
            var substring = string.Join(' ', words[0..i]);
            if (TimeSpanParser.TryParse(substring, out var ts) && ts == timeSpan)
            {
                continue;
            }

            return string.Join(' ', words[(i + 1)..]);
        }

        return string.Empty;
    }

    private static string Humanize(TimeSpan ts)
        => ts.Humanize(precision: 3, minUnit: Humanizer.Localisation.TimeUnit.Second);
}
