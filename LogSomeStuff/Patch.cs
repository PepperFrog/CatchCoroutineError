using MEC;
using UnityEngine;

namespace LogSomeStuff;

[HarmonyPatch]
public static class Patch
{
    static int Id = 0;
    public static List<(CoroutineHandle handler, int id)> coroutines = new List<(CoroutineHandle handler, int id)>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Timing), nameof(Timing.RunCoroutine))]
    public static void GetCoroutineStarted(ref CoroutineHandle __result)
    {
        coroutines.Add((__result, Id++));
        StackTrace trace = new StackTrace();
        Log.Warn(trace.ToString());
    }

}
