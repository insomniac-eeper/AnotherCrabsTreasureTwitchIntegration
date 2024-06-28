/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public record TimedEffectStateRecord(
    string EffectId,
    long RequestedTime,
    string RequestedBy,
    string EffectType,
    bool IsActive,
    long? Duration,
    long? DurationLeft,
    long? ActivationTime) :
        EffectStateRecord(
            EffectId,
            RequestedTime,
            RequestedBy,
            ActivationTime);