/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class Die() : EffectDefinition("die",
    "Die",
    "Die",
    600,
    DoEffect)
{
    private static bool DoEffect()
    {
        //GameObject.Find("Player_New(Clone)").GetComponent<Player>().Die();
        Player.singlePlayer.Die(null);
        return true;
    }
}