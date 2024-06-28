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
public class InvincibilityPatches
{
    public static readonly ConcurrentStack<bool> InvincibilityStack = new();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TakeDamagePlayer))]
    public static bool Player_TakeDamagePlayer_Prefix(ref HitEvent e)
    {
        var damage = InvincibilityStack.Any() ? 0f : e.damage;
        e = new HitEvent(
            e.target, e.source, damage, e.knockback, e.contactPoint, e.hitWeight, e.damageType, e.hurtbox, e.hitbox,
            e.afflictionType, e.afflictionDamage, e.extraPoise, e.poiseMulti, e.hitboxTag);

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TakeShellDamage))]
    public static bool Player_TakeShellDamage_Prefix(ref float damage)
    {
        damage = InvincibilityStack.Any() ? 0 : damage;
        return true;
    }

}