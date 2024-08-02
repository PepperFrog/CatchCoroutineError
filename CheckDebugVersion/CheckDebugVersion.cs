using System.Reflection;
using Exiled.API.Features;

namespace CheckDebugVersion;

public class CheckDebugVersion : Plugin<CheckDebugVersion.PluginConfig>
{

    private Harmony Harmony { get; set; }

    public override string Name { get; } = PluginInfo.PLUGIN_NAME;

    public override string Prefix { get; } = PluginInfo.PLUGIN_NAME;

    public override string Author { get; } = PluginInfo.PLUGIN_AUTHORS;

    public override PluginPriority Priority { get; } = PluginPriority.First;

    public override Version Version { get; } = new Version(PluginInfo.PLUGIN_VERSION);

    public override void OnEnabled()
    {
        var pluginsToLogs = Loader.Plugins.Where(p => IsAssemblyDebugBuild(p.Assembly));

        if (pluginsToLogs.Any())
        {
            Log.Error(@$"
CES PLUGINS SUIVENT PEUVE ÊTRES LA CAUSE DE RALENTISMENTS DU SERVER !
Les plugins compilers en mode débug ne sont pas optimisés pour la production.
Veuillez contacter les auteurs des plugins suivants pour obtenir une version optimisée:
{HumainReadablePlugins(pluginsToLogs)}");
        }

        base.OnEnabled();
    }

    public string HumainReadablePlugins(IEnumerable<IPlugin<IConfig>> plugins)
    {
        string message = "";
        foreach (var plugin in plugins)
        {
            message += $"- {plugin.Name} ({plugin.Version}), contacter => {plugin.Author}\n";
        }
        return message;
    }

    public bool IsAssemblyDebugBuild(Assembly assembly)
    {
        return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
    }

    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get => true; set { } }
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

