/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Extensions;
using Patches.Effect;
using Types;

public class Invincibility():
    TimedEffectDefinition(
        "invincibility",
        "Invincibility",
        "Make Kril invincible for a short time",
        cooldownInSeconds: 10,
        duration: 10,
        DoStartEffect,
        DoEndEffect)
{

    private static bool DoStartEffect()
    {
        InvincibilityPatches.InvincibilityStack.Push(true);
        return true;
    }

    private static bool DoEndEffect()
    {

        InvincibilityPatches.InvincibilityStack.SpinTryPop(out _);
        return true;
    }
}