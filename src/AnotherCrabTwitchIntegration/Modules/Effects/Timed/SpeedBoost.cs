/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Extensions;
using Patches.Effect;
using Types;

public class SpeedBoost() :
    TimedEffectDefinition(
        "speedboost",
        "Speed Boost",
        "Speeds up Kril",
        cooldownInSeconds: 0,
        duration: 20,
        DoStartEffect,
        DoEndEffect)
{

    private static bool DoStartEffect()
    {
        MoveSpeedModPatches.SpeedBoostStack.Push(1.5f);
        return true;
    }

    private static bool DoEndEffect()
    {
        MoveSpeedModPatches.SpeedBoostStack.SpinTryPop(out _);
        return true;
    }
}