/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Patches.Effect;
using Types;

public class NoJumping():
    TimedEffectDefinition(
        "nojumping",
        "Disable Jumping",
        "Make Kril unable to jump for a short time",
        cooldownInSeconds: 10,
        duration: 10,
        DoStartEffect,
        DoEndEffect)
{

    private static bool DoStartEffect()
    {
        PlayerJumpPatches.CanJump = false;
        return true;
    }

    private static bool DoEndEffect()
    {
        PlayerJumpPatches.CanJump = true;
        return true;
    }
}