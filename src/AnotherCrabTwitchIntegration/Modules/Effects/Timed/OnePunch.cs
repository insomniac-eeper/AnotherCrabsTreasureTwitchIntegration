/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using System;
using Patches.Effect;
using Types;

public class OnePunch : TimedEffectDefinition
{
    private Guid _modifierId;

    public OnePunch() : base(
        "onepunch",
        "One Hit Kill",
        "Make Kril kill any enemy with one hit for a short time",
        cooldownInSeconds: 10,
        duration: 10,
        startEffect: null,
        endEffect: null)
    {
        OnStartEffect += DoStartEffect;
        OnEndEffect += DoEndEffect;
    }

    private bool DoStartEffect()
    {
        _modifierId = DamageModPatches.AddDamageMod(9999f);
        return true;
    }

    private bool DoEndEffect()
    {
        DamageModPatches.RemoveDamageMod(_modifierId);
        return true;
    }
}