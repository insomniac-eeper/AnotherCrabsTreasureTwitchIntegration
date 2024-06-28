/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class StartAfk() : EffectDefinition(
    "startafk",
    "Start AFK Emote",
    "Start AFK Emote",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        Player.singlePlayer.StartHackySackAfk();
        return true;
    }
}