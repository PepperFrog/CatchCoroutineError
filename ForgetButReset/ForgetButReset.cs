namespace ForgetButReset;

public class ForgetButReset : Plugin<ForgetButReset.PluginConfig>
{
    public override string Name { get; } = PluginInfo.PLUGIN_NAME;

    public override string Prefix { get; } = PluginInfo.PLUGIN_NAME;

    public override string Author { get; } = PluginInfo.PLUGIN_AUTHORS;

    public override PluginPriority Priority { get; } = PluginPriority.First;

    public override Version Version { get; } = new Version(PluginInfo.PLUGIN_VERSION);

    public override void OnEnabled()
    {
        ServerEvents.WaitingForPlayers.Subscribe(OnWaiting);

        base.OnEnabled();
    }

    private void OnWaiting()
    {
        Server.FriendlyFire = Config.FrendlyFireDefault;
        Round.IsLobbyLocked = false;
        Round.IsLocked = false;
    }

    public override void OnDisabled()
    {
        ServerEvents.RestartingRound.Unsubscribe(OnWaiting);

        base.OnDisabled();
    }

    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = false;
        
        public bool FrendlyFireDefault { get; set; } = false;

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
