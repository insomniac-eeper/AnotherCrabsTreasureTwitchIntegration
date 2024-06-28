/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Types;

public class Invisible() : TimedEffectDefinition(
    "invisible",
    "Invisible",
    "Make Kril invisible for a short time",
    cooldownInSeconds: 10,
    duration: 10,
    DoStartEffect,
    DoEndEffect)
{
    private static bool DoStartEffect()
    {
        CameraController.instance.TurnPlayerInvisible(invisible: true);
        return true;
    }

    private static bool DoEndEffect()
    {
        CameraController.instance.TurnPlayerInvisible(invisible: false);
        return true;
    }
}