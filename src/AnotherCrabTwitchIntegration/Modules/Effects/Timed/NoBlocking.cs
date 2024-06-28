/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Patches.Effect;
using Types;

public class NoBlocking():
    TimedEffectDefinition(
        "noblocking",
        "Disable Blocking",
        "Make Kril unable to block for a short time",
        cooldownInSeconds: 10,
        duration: 10,
        DoStartEffect,
        DoEndEffect)
{

    private static bool DoStartEffect()
    {
        PlayerBlockPatches.CanBlock = false;
        return true;
    }

    private static bool DoEndEffect()
    {
        PlayerBlockPatches.CanBlock = true;
        return true;
    }
}