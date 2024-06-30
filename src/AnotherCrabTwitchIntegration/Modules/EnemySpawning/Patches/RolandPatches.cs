/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

using System.Reflection;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

[HarmonyPatch]
public static class RolandPatches
{
    private static bool OnRolandDie(Roland roland)
    {
        Plugin.Log.LogWarning($"CustomSpawn on Roland: {roland.gameObject.GetComponent<CustomSpawn>() != null}");
        return roland.gameObject.GetComponent<CustomSpawn>() != null;
    }

    private static readonly MethodInfo s_onRolandDie = AccessTools.Method(typeof(RolandPatches), nameof(OnRolandDie));

    /// <summary>
    /// Skip AchievementThrower.SetValue call if the Inkerton has CustomSpawn component
    /// </summary>
    /// <param name="il"></param>
    [HarmonyILManipulator]
    [HarmonyPatch(typeof(Roland), nameof(Roland.Die))]
    public static void OnDestroyIl(ILContext il)
    {
        ILCursor c = new(il);


        c.GotoNext(MoveType.Before,
            x => x.MatchCall(AccessTools.PropertyGetter(typeof(GameManager), nameof(GameManager.instance)))
        );

        var beforeSaveProgress = c.MarkLabel();

        c.GotoNext(MoveType.Before,
            x => x.MatchCall(AccessTools.PropertyGetter(typeof(Component), nameof(Component.gameObject)))
        );

        // Ensure we get loadarg too
        c.Index--;
        var beforeDestroyLabel = c.MarkLabel();

        c.GotoNext(MoveType.Before,
            x => x.MatchCall(AccessTools.PropertyGetter(typeof(DialogueManager), nameof(DialogueManager.instance)))
        );

        var beforeDialogManager = c.MarkLabel();

        // Find the return statement
        c.GotoNext(MoveType.Before,
            x => x.MatchRet());

        var endLabel = c.MarkLabel();
        c.GotoLabel(beforeSaveProgress);

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, s_onRolandDie);
        c.Emit(OpCodes.Brtrue, beforeDestroyLabel);

        c.GotoLabel(beforeDialogManager);

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, s_onRolandDie);
        c.Emit(OpCodes.Brtrue, endLabel);
    }
}
