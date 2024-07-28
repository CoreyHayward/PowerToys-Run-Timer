// Copyright (c) Corey Hayward. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.Timers;

public class Main : IPlugin, IContextMenu, ISettingProvider
{
    private List<TimerPlus> _timers = [];
    private TimerResultService? _timerService;
    private PluginInitContext? _context;
    private Settings _settings = new();

    public string Name => "Timer";

    public string Description => "Set and manage timers.";

    public static string PluginID => "0cd68088246649a38205c7641df39db0";

    public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
    {
        new PluginAdditionalOption()
        {
            Key = nameof(Settings.TimeSpanParserUncolonedDefault),
            DisplayLabel = "Uncoloned Time Configuration",
            DisplayDescription = "Sets how the first number of the timer should be treated, when not using colons.",
            ComboBoxValue = (int)_settings.TimeSpanParserUncolonedDefault,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
            ComboBoxItems = Settings.TimeSpanParserConfigurationOptions
        },
        new PluginAdditionalOption()
        {
            Key = nameof(Settings.TimeSpanParserColonedDefault),
            DisplayLabel = "Coloned Time Configuration",
            DisplayDescription = "Sets how the first number of the timer should be treated, when using colons (i.e. 4:30).",
            ComboBoxValue = (int)_settings.TimeSpanParserColonedDefault,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
            ComboBoxItems = Settings.TimeSpanParserConfigurationOptions
        },
    };

    public void Init(PluginInitContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
        _timerService = new(_context);
    }

    public List<Result> Query(Query query)
        => _timerService!.GetQueryResult(query, _settings);

    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        => _timerService!.GetContextMenuResults(selectedResult);

    public System.Windows.Controls.Control CreateSettingPanel()
        => throw new NotImplementedException();

    public void UpdateSettings(PowerLauncherPluginSettings settings)
    {
        if (settings?.AdditionalOptions is null)
        {
            return;
        }

        _settings.UpdateSettings(settings.AdditionalOptions);
    }
}
