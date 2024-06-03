
using Humanizer;
using ManagedCommon;
using System.Timers;
using System.Windows.Input;
using System.Xml.Linq;
using TimeSpanParserUtil;
using Wox.Plugin;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Community.PowerToys.Run.Plugin.Timers;
public class TimerResultService
{
    private readonly List<TimerPlus> _timers = [];
    private readonly PluginInitContext _pluginContext;
    private string _iconPath = "Images/Timer.light.png";

    public TimerResultService(PluginInitContext pluginContext)
    {
        ArgumentNullException.ThrowIfNull(pluginContext);
        _pluginContext = pluginContext;
        _pluginContext.API.ThemeChanged += (_, theme) => UpdateTheme(theme);
        UpdateTheme(_pluginContext.API.GetCurrentTheme());
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

    public List<Result> GetQueryResult(Query query)
    {
        if (string.IsNullOrWhiteSpace(query.Search))
        {
            return GetRunningTimersResults(query.Search);
        }

        return GetCreateTimerResult(query.Search);
    }

    private List<Result> GetRunningTimersResults(string search)
    {
        var timerResults = new List<Result>();
        for (var i = 0; i < _timers.Count; i++)
        {
            var timer = _timers[i];
            var timerInterval = TimeSpan.FromMilliseconds(timer.Interval);
            var resultTitle = string.IsNullOrWhiteSpace(timer.Title)
                ? $"{i + 1}: {Humanize(timerInterval).Singularize(false)} timer"
                : $"{i + 1}: {timer.Title} ({Humanize(timerInterval).Singularize(false)} timer)";

            timerResults.Add(new Result()
            {
                QueryTextDisplay = search,
                Title = resultTitle,
                SubTitle = $"{Humanize(timer.TimeLeft)} left",
                IcoPath = _iconPath,
                Action = _ => true,
                ContextData = timer,
            });
        }

        return timerResults;
    }

    public List<Result> GetCreateTimerResult(string query)
    {
        query = query.Trim();
        if (!TimeSpanParser.TryParse(query, out var timeSpan) || timeSpan <= TimeSpan.Zero)
        {
            var parsingErrorResult = new Result()
            {
                QueryTextDisplay = query,
                Title = "Parsing error",
                SubTitle = "Unable to parse the provided time. Try using one of the following formats: 30s, 15m or 1h or 2h30m",
                IcoPath = _iconPath,
                Action = _ => true,
            };

            return [parsingErrorResult];
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
                var timer = new TimerPlus(timeSpan, timerTitle);
                timer.AutoReset = false;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                _timers.Add(timer);
                return true;
            }
        };

        return [result];

        void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var timer = (TimerPlus)sender!;
            var timeSpan = TimeSpan.FromMilliseconds(timer.Interval);
            timer.Dispose();

            _timers.Remove(timer);
            if (string.IsNullOrWhiteSpace(timer.Title))
            {
                _pluginContext!.API.ShowNotification($"{Humanize(timeSpan).Singularize(false)} timer elapsed.");
            }
            else
            {
                _pluginContext!.API.ShowNotification(timer.Title, $"{Humanize(timeSpan).Singularize(false)} timer elapsed.");
            }
        }
    }

    public List<ContextMenuResult> GetContextMenuResults(Result selectedResult)
    {
        var timer = selectedResult.ContextData as TimerPlus;
        if (timer is null) return [];

        var deleteTimer = new ContextMenuResult()
        {
            Title = "Delete Timer (Ctrl+Enter)",
            Glyph = "\xE74D",
            FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
            AcceleratorKey = Key.Enter,
            AcceleratorModifiers = ModifierKeys.Control,
            Action = _ =>
            {
                timer.Dispose();
                _timers.Remove(timer);
                _pluginContext.API.RemoveUserSelectedItem(selectedResult);
                return true;
            },
        };

        return [deleteTimer];
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
