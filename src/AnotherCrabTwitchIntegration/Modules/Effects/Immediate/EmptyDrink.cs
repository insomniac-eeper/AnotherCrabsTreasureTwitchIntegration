/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class EmptyDrink() : EffectDefinition(
    "emptydrink",
    "Empty Heartkelp Sprouts",
    "Empty Heartkelp Sprouts.",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        var player = Player.singlePlayer;
        if (player.healthyEggCount == 0)
        {
            return false;
        }

        player.healthyEggCount = 0;
        return true;
    }
}