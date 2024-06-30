using System.Reflection;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

[HarmonyPatch]
public static class TopodaPatches
{
    private static bool OnTopodaDie(Topoda topoda)
    {
        return topoda.gameObject.GetComponent<CustomSpawn>() != null;
    }

    private static readonly MethodInfo s_onTopodaDie = AccessTools.Method(typeof(TopodaPatches), nameof(OnTopodaDie));

    [HarmonyILManipulator]
    [HarmonyPatch(typeof(Topoda), nameof(Topoda.Die))]
    private static void OnDieIL(ILContext il)
    {
        ILCursor c = new(il);

        // Find the first call to CrabFile.current indicating the start of the logic which affects save state and achievements
        c.GotoNext(MoveType.Before,
            x => x.MatchCall(AccessTools.PropertyGetter(typeof(CrabFile), "current"))
        );
        var targetInsertion = c.MarkLabel();

        // Find the return statement
        c.GotoNext(MoveType.Before,
            x => x.MatchRet());

        var endLabel = c.MarkLabel();
        c.GotoLabel(targetInsertion);

        // Call s_onTopodaDie with the Topoda instance as argument
        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, s_onTopodaDie);
        c.Emit(OpCodes.Brtrue, endLabel);
    }
}
