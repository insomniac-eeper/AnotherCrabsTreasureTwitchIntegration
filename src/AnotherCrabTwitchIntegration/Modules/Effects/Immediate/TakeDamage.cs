/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class TakeDamage() :
    EffectDefinition(
        "takedamage",
        "Take Damage",
        "Take damage",
        15,
        DoEffect)
{
    private static bool DoEffect()
    {
        Player.singlePlayer.TakeDamage(10);
        return true;
    }
}
