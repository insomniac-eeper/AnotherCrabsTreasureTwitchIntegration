/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using System.Collections.Concurrent;
using System.Linq;
using Types;

public class EffectStateSnapshotter(
    int snapShotIntervalInMilliSeconds = 100,
    bool debugSnapshotLogOutput = false,
    ConcurrentQueue<IEffect>? activatedEffects = default,
    ConcurrentDictionary<Guid, IEffect>? activeEffects = default,
    ConcurrentDictionary<string, float>? cooldowns = default,
    ConcurrentDictionary<ITimedEffect, float>? durations = default,
    ConcurrentQueue<IEffect>? queuedEffects = default)
{
    private long _lastUpdateTime;

    public int EffectSnapShotIntervalInMilliSeconds { get; set; } = snapShotIntervalInMilliSeconds;
    public bool DebugSnapshotLogOutput { get; set; } = debugSnapshotLogOutput;

    public Action<EffectManagerStateSnapshotRecord>? OnSnapshot = null;

    private EffectStateRecord GetEffectStateRecord(IEffect effect)
    {
        if (effect is ITimedEffect timedEffect)
        {
            return GetEffectStateRecord(timedEffect);
        }

        return new EffectStateRecord(
            effect.Definition.Id,
            effect.RequestTimeStamp,
            effect.Requester,
            effect.ActivationTimeStamp
        );
    }

    private TimedEffectStateRecord GetEffectStateRecord(ITimedEffect effect)
    {
        return new TimedEffectStateRecord(
            EffectId: effect.Definition.Id,
            RequestedTime: effect.RequestTimeStamp,
            RequestedBy: effect.Requester,
            EffectType: nameof(ITimedEffect),
            IsActive: effect.IsActive(),
            Duration: effect.TimedDefinition.Duration,
            // TODO: This looks messy... reconsider making durations and other collections nullable
            DurationLeft: durations != null && durations.TryGetValue(effect, out float duration) ? (long?)duration : null,
            ActivationTime: effect.ActivationTimeStamp
        );
    }

    private EffectManagerStateSnapshotRecord GetSnapshot()
    {
        var cooldownState = cooldowns?.Where(pair => pair.Value > 0).ToDictionary(pair => pair.Key, pair => pair.Value);
        return new EffectManagerStateSnapshotRecord(
            (queuedEffects ?? []).ToList().Select(GetEffectStateRecord).ToList(),
            (activeEffects ?? []).Values.ToList().Select(GetEffectStateRecord).ToList(),
            (activatedEffects ?? []).ToList().Select(GetEffectStateRecord).ToList(),
            cooldownState ?? []
        );
    }

    private void DebugPrintSnapshot(EffectManagerStateSnapshotRecord snapshot)
    {
        Plugin.Log.LogInfo("-------------Snapshot----------------");
        Plugin.Log.LogInfo($"Queued: {snapshot.QueuedEffects.Count}");
        foreach (var queuedEffect in snapshot.QueuedEffects)
        {
            Plugin.Log.LogInfo($"    - {queuedEffect}");
        }

        Plugin.Log.LogInfo($"Active: {snapshot.ActiveEffects.Count}");
        foreach (var activeEffect in snapshot.ActiveEffects)
        {
            Plugin.Log.LogInfo($"    - {activeEffect}");
        }

        Plugin.Log.LogInfo($"Cooldowns: {snapshot.Cooldowns.Count}");
        foreach (var cooldown in snapshot.Cooldowns)
        {
            Plugin.Log.LogInfo($"    - {cooldown.Key}: {cooldown.Value}");
        }

        Plugin.Log.LogInfo($"RecentlyActivated: {snapshot.RecentlyActivatedEffects.Count}");
        foreach (var recentlyActivatedEffect in snapshot.RecentlyActivatedEffects)
        {
            Plugin.Log.LogInfo($"    - {recentlyActivatedEffect}");
        }
    }

    internal void Update()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - _lastUpdateTime <= EffectSnapShotIntervalInMilliSeconds) return;

        _lastUpdateTime = currentTime;
        var snapshot = GetSnapshot();
        OnSnapshot?.Invoke(snapshot);

        if (DebugSnapshotLogOutput)
        {
            DebugPrintSnapshot(snapshot);
        }
    }
}
