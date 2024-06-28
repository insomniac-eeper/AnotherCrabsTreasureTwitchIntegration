/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public record EffectStateRecord(
    string EffectId,
    long RequestedTime,
    string RequestedBy,
    long? ActivationTime
    );