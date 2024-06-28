/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches.Effect;

using System.Threading;
using HarmonyLib;

[HarmonyPatch]
public class PlayerRunPatches
{
    public static bool CanRun
    {
        get => _canRun == 1;
        set
        {
            var newVal = value ? 1 : 0;
            Interlocked.Exchange(ref _canRun, newVal);
        }
    }

    private static int _canRun = 1;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.canSprint), MethodType.Getter)]
    public static void Player_canSprint_Postfix(ref bool __result)
    {
        __result = CanRun;
    }

}