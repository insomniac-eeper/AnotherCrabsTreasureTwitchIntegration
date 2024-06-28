/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class DrinkOne() : EffectDefinition(
    "drinkone",
    "Drink One Heartkelp",
    "Drink one Heartkelp.",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        var player = Player.singlePlayer;
        player.Drink();
        return true;
    }
}