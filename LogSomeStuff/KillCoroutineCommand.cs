using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using MEC;

namespace LogSomeStuff;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class KillCoroutineCommand : ICommand
{
    public string Command => nameof(KillCoroutineCommand);

    public string[] Aliases => [];

    public string Description => "Kill a courtine.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        try
        {
            // Timing.CurrentCoroutine
            if (Patch.coroutines.Count == 0)
            {
                response = "None to kill";
                return true;
            }

            (CoroutineHandle handler, int id) = Patch.coroutines.FirstOrDefault();

            var killed = Timing.CurrentCoroutine;
            response = $"{Timing.GetDebugName(handler)} get kill {id}";
            Timing.KillCoroutines(handler);
            return true;
        }
        catch (Exception e)
        {
            response = e.ToString();
            return false;
        }

    }
}