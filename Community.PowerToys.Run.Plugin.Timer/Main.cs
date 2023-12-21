// Copyright (c) Corey Hayward. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.Timers;

public class Main : IPlugin, IContextMenu
{
    private List<TimerPlus> _timers = [];
    private TimerResultService? _timerService;
    private PluginInitContext? _context;

    public string Name => "Timer";

    public string Description => "Timers.";

    public static string PluginID => "0cd68088246649a38205c7641df39db0";

    public void Init(PluginInitContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
        _timerService = new(_context);
    }

    public List<Result> Query(Query query)
        => _timerService!.GetQueryResult(query);

    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        => _timerService!.GetContextMenuResults(selectedResult);
}
