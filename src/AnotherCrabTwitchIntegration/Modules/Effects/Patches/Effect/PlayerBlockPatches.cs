/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches.Effect;

using System.Threading;
using HarmonyLib;

[HarmonyPatch]
public class PlayerBlockPatches
{
    private static int s_canBlock = 1;
    public static bool CanBlock
    {
        get => s_canBlock == 1;
        set
        {
            int newVal = value ? 1 : 0;
            Interlocked.Exchange(ref s_canBlock, newVal);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.canBlock), MethodType.Getter)]
    public static bool Player_Block_Prefix()
    {
        return CanBlock;
    }
}
