using AdminToys;
using Exiled.API.Features.Toys;
using MEC;
using OptiPrimitves;

namespace OptiPrimitves;

public class OptiPrimitves : Plugin<OptiPrimitves.PluginConfig>
{
    public override string Name { get; } = PluginInfo.PLUGIN_NAME;

    public override string Prefix { get; } = PluginInfo.PLUGIN_NAME;

    public override string Author { get; } = PluginInfo.PLUGIN_AUTHORS;

    public override PluginPriority Priority { get; } = PluginPriority.First;

    public override Version Version { get; } = new Version(PluginInfo.PLUGIN_VERSION);

    public override void OnEnabled()
    {
        ServerEvents.RoundStarted.Subscribe(Start);

        base.OnEnabled();
    }

    private IEnumerator<float> Start()
    {
        yield return Timing.WaitForOneFrame;

        foreach (var toy in UnityEngine.Object.FindObjectsOfType<AdminToyBase>())
        {
            toy.NetworkIsStatic = true;
            toy.gameObject.SetActive(false);
        }
    }

    public override void OnDisabled()
    {
        ServerEvents.RoundStarted.Subscribe(Start);

        base.OnDisabled();
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
