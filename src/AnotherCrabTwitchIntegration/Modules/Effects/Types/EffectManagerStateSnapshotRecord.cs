/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

using System.Collections.Generic;

public record struct EffectManagerStateSnapshotRecord(
    List<EffectStateRecord> QueuedEffects,
    List<EffectStateRecord> ActiveEffects,
    List<EffectStateRecord> RecentlyActivatedEffects,
    Dictionary<string, float> Cooldowns
    );