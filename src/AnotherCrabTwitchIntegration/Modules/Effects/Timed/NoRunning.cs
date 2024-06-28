/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Patches.Effect;
using Types;

public class NoRunning():
    TimedEffectDefinition(
        "norunning",
        "Disable Running",
        "Make Kril unable to run for a short time",
        cooldownInSeconds: 10,
        duration: 10,
        DoStartEffect,
        DoEndEffect)
{

    private static bool DoStartEffect()
    {
        PlayerRunPatches.CanRun = false;
        return true;
    }

    private static bool DoEndEffect()
    {
        PlayerRunPatches.CanRun = true;
        return true;
    }
}