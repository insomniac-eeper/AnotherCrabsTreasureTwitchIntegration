/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public class OverridenEffect(
    EffectDefinition definition,
    string nameOverride = null,
    string descriptionOverride = null,
    int? cooldownInSecondsOverride = null)
    : EffectDefinition(definition.Id,
        nameOverride ?? definition.Name,
        descriptionOverride ?? definition.Description,
        cooldownInSecondsOverride ?? definition.CooldownInSeconds,
        definition.ApplyEffect);