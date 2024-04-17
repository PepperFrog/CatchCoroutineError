using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MEC;

namespace CatchCoroutineError;

public class CatchCoroutineError : Plugin<Config>
{

    private Harmony Harmony { get; set; }
    
    public override string Name { get; } = PluginInfo.PLUGIN_NAME;

    public override string Prefix { get; } = PluginInfo.PLUGIN_NAME;

    public override string Author { get; } = PluginInfo.PLUGIN_AUTHORS;

    public override PluginPriority Priority { get; } = PluginPriority.First;

    public override Version Version { get; } = new Version(PluginInfo.PLUGIN_VERSION);

    public override void OnEnabled()
    {
        Harmony.DEBUG = Config.Debug;
        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll();
    }
    public override void OnDisabled()
    {
        Harmony.UnpatchAll();
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

[HarmonyPatch]
internal static class ReplaceErrorLogMethods
{

    [HarmonyTargetMethods]
    internal static IEnumerable<MethodBase> AllTimingMethods()
    {
        yield return AccessTools.Method(typeof(Timing), "Update");
        yield return AccessTools.Method(typeof(Timing), "FixedUpdate");
        yield return AccessTools.Method(typeof(Timing), "LateUpdate");
        yield return AccessTools.Method(typeof(Timing), "RunCoroutineInternal");
    }

    [HarmonyTranspiler]
    internal static IEnumerable<CodeInstruction> ReplaceLogMethod(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var codeMatcher = new CodeMatcher(instructions);
        
        var obj = new object();
        codeMatcher.MatchStartForward(
            new CodeMatch((inst) => inst.opcode == OpCodes.Call && inst.operand is MethodInfo { Name: "LogError" })
            );

        codeMatcher.Repeat((match) =>
        {
            match.InsertCopyCall(() => Log.Error(obj));
        });

        codeMatcher.Start();

        var exeption = new Exception();
        codeMatcher.MatchStartForward(
            new CodeMatch((inst) => inst.opcode == OpCodes.Call && inst.operand is MethodInfo { Name: "LogException" })
            );

        codeMatcher.Repeat((match) =>
        {
            match.InsertCopyCall(() => Log.Error(exeption));
        });

        return codeMatcher.InstructionEnumeration();

    }

    private static void InsertCopyCall(this CodeMatcher match, Expression<Action> expression)
    {
        // copy the labels and blocks in case if the method is first instrution of a try catch, if/else...
        ref var labels = ref match.Labels;
        ref var blocks = ref match.Blocks;
        match.Insert(new CodeInstruction(OpCodes.Dup));
        match.Labels.AddRange(labels);
        labels.Clear();
        match.Blocks.AddRange(blocks);
        blocks.Clear();
        match.Advance(1);
        match.InsertAndAdvance(CodeInstruction.Call(expression));
        // Pass over the orignal method
        match.Advance(1);
    }
}