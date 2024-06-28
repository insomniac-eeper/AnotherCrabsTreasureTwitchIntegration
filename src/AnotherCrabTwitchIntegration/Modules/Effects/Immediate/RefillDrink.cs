/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class RefillDrink() : EffectDefinition(
    "refilldrink",
    "Refill Heartkelp Sprouts",
    "Refill Heartkelp Sprouts.",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        var player = Player.singlePlayer;
        player.ResetDrink();
        return true;
    }
}