/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches.Effect;

using System.Threading;
using HarmonyLib;

[HarmonyPatch]
public class PlayerJumpPatches
{
    private static int s_canJump = 1;
    public static bool CanJump
    {
        get => s_canJump == 1;
        set
        {
            int newVal = value ? 1 : 0;
            Interlocked.Exchange(ref s_canJump, newVal);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
    public static bool Player_Jump_Prefix()
    {
        return CanJump;
    }
}
