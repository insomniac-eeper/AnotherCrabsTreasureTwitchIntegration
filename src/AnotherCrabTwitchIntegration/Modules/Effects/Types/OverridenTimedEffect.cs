/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public class OverridenTimedEffect(
    TimedEffectDefinition definition,
    string? nameOverride = null,
    string? descriptionOverride = null,
    int? cooldownInSecondsOverride = null,
    int? durationOverride = null)
        : TimedEffectDefinition(
            id: definition.Id,
            name: nameOverride ?? definition.Name,
            description: descriptionOverride ?? definition.Description,
            cooldownInSeconds: cooldownInSecondsOverride ?? definition.CooldownInSeconds,
            duration: durationOverride ?? definition.Duration,
            startEffect: definition.ApplyEffect,
            endEffect: definition.EndEffect);
