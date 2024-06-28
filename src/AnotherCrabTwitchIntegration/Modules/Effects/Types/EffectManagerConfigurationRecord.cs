/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public record EffectManagerConfigurationRecord(
    int SnapshotIntervalInMilliSeconds,
    bool DebugSnapshotLogOutput
)
{
    static EffectManagerConfigurationRecord()
    {
        Default = new(1, false);
    }

    public static readonly EffectManagerConfigurationRecord Default;
}