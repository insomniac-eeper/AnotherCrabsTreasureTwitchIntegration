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
    private static int _canBlock = 1;
    public static bool CanBlock
    {
        get => _canBlock == 1;
        set
        {
            var newVal = value ? 1 : 0;
            Interlocked.Exchange(ref _canBlock, newVal);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.canBlock), MethodType.Getter)]
    public static bool Player_Block_Prefix()
    {
        return CanBlock;
    }
}