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
    private static int _canJump = 1;
    public static bool CanJump
    {
        get => _canJump == 1;
        set
        {
            var newVal = value ? 1 : 0;
            Interlocked.Exchange(ref _canJump, newVal);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
    public static bool Player_Jump_Prefix()
    {
        return CanJump;
    }
}