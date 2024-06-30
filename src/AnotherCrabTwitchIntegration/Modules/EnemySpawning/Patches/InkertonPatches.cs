/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

using System.Reflection;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

public static class InkertonPatches
{
    private static bool OnInkertonDestroy(Topoda topoda)
    {
        return topoda.gameObject.GetComponent<CustomSpawn>() != null;
    }

    private static readonly MethodInfo s_onInkertonDestroy = AccessTools.Method(typeof(TopodaPatches), nameof(OnInkertonDestroy));

    /// <summary>
    /// Skip AchievementThrower.SetValue call if the Inkerton has CustomSpawn component
    /// </summary>
    /// <param name="il"></param>
    [HarmonyILManipulator]
    [HarmonyPatch(typeof(Inkerton), nameof(Inkerton.OnDestroy))]
    public static void OnDestroyIl(ILContext il)
    {
        ILCursor c = new(il);

        c.GotoNext(MoveType.Before,
            x => x.MatchCall(AccessTools.PropertyGetter(typeof(AchievementThrower), nameof(AchievementThrower.SetValue)))
        );
        var targetInsertion = c.MarkLabel();

        // Find the return statement
        c.GotoNext(MoveType.Before,
            x => x.MatchRet());

        var endLabel = c.MarkLabel();
        c.GotoLabel(targetInsertion);

        // Call s_onTopodaDie with the Topoda instance as argument
        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, s_onInkertonDestroy);
        c.Emit(OpCodes.Brtrue, endLabel);
    }
}
