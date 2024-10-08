﻿using AdminToys;
using MEC;

namespace LogSomeStuff;

public class LogSomeStuff : Plugin<LogSomeStuff.PluginConfig>
{
    public override string Name { get; } = PluginInfo.PLUGIN_NAME;

    public override string Prefix { get; } = PluginInfo.PLUGIN_NAME;

    public override string Author { get; } = PluginInfo.PLUGIN_AUTHORS;

    public override PluginPriority Priority { get; } = PluginPriority.First;

    public override Version Version { get; } = new Version(PluginInfo.PLUGIN_VERSION);

    public override void OnEnabled()
    {
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
    }

    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug
        {
            get =>
#if DEBUG
                    true;
#else
                    false;
#endif
            set { }
        }
    }

}
