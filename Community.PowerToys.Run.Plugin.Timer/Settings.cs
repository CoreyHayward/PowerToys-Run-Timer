using Microsoft.PowerToys.Settings.UI.Library;
using TimeSpanParserUtil;

namespace Community.PowerToys.Run.Plugin.Timers;

public class Settings
{
    public static List<KeyValuePair<string, string>> TimeSpanParserConfigurationOptions
        = new List<KeyValuePair<string, string>>()
            {
                new("None", ((int)Units.None).ToString()),
                new("Milliseconds", ((int)Units.Milliseconds).ToString()),
                new("Seconds", ((int)Units.Seconds).ToString()),
                new("Minutes", ((int)Units.Minutes).ToString()),
                new("Hours", ((int)Units.Hours).ToString()),
                new("Days", ((int)Units.Days).ToString()),
                new("Weeks", ((int)Units.Weeks).ToString()),
                new("Months", ((int)Units.Months).ToString()),
            };

    public TimeSpanParserOptions TimeSpanParserOptions => new()
    {
        UncolonedDefault = TimeSpanParserUncolonedDefault,
        ColonedDefault = TimeSpanParserColonedDefault,
    };

    public Units TimeSpanParserColonedDefault { get; private set; } = Units.Hours;
    public Units TimeSpanParserUncolonedDefault { get; private set; } = Units.Minutes;

    public void UpdateSettings(IEnumerable<PluginAdditionalOption> settings)
    {
        TimeSpanParserUncolonedDefault = (Units)settings.Single(x => x.Key == nameof(Settings.TimeSpanParserUncolonedDefault)).ComboBoxValue;
        TimeSpanParserColonedDefault = (Units)settings.Single(x => x.Key == nameof(Settings.TimeSpanParserColonedDefault)).ComboBoxValue;
    }
}
