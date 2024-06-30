/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches.Effect;

using System.Collections.Concurrent;
using System.Linq;
using HarmonyLib;

[HarmonyPatch]
public class MoveSpeedModPatches
{
    public static readonly ConcurrentStack<float> SpeedBoostStack = new();
    public static readonly ConcurrentStack<float> SpeedReductionStack = new();

    [HarmonyPatch(typeof(Player), nameof(Player.currentWalkAcceleration), MethodType.Getter)]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void Player_currentWalkAcceleration_Postfix(ref float __result)
    {
        float speedMultiplier = SpeedBoostStack.Aggregate(1f, (a, b) => a * b);
        speedMultiplier = SpeedReductionStack.Aggregate(speedMultiplier, (a, b) => a * b);
        __result *= speedMultiplier;
    }


}
