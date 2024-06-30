/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches.Effect;

using System;
using System.Collections.Concurrent;
using System.Linq;
using HarmonyLib;

[HarmonyPatch]
public class DamageModPatches
{
    private static readonly ConcurrentDictionary<Guid, float> s_damageModValues = new();

    [HarmonyPatch(typeof(Player), nameof(Player.OnSuccessfulHit))]
    [HarmonyPostfix]
    public static void Player_currentWalkAcceleration_Postfix(ref HitEvent e)
    {
        e.baseDamageAdditive += s_damageModValues.Values.Sum();
    }

    public static Guid AddDamageMod(float damageMod)
    {
        var id = Guid.NewGuid();
        s_damageModValues[id] = damageMod;
        return id;
    }

    public static void RemoveDamageMod(Guid id)
    {
        while(s_damageModValues.ContainsKey(id))
        {
            s_damageModValues.TryRemove(id, out _);
        }
    }

}
