/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class SetHpToOne() : EffectDefinition(
    "sethptoone",
    "Set HP to 1",
    "Set HP to 1",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        var player = Player.singlePlayer;
        float playerHealth = player.health;
        float damageToTake = playerHealth - 1;
        if (damageToTake <= 0)
        {
            return false;
        }

        Player.singlePlayer.health = 1;
        GUIManager.instance.HUD.healthBar.SetAmount(1);
        return true;
    }
}
