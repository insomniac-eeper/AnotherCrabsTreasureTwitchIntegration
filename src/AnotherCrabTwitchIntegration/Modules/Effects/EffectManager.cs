/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Patches;
using Types;

public class EffectManager
{
    private readonly EffectManagerConfigurationRecord _configuration;

    private readonly ConcurrentQueue<IEffect> _activatedEffects = [];
    private readonly ConcurrentDictionary<Guid, IEffect> _activeEffects = [];
    private readonly ConcurrentDictionary<string, float> _cooldowns = new();
    private readonly ConcurrentDictionary<ITimedEffect, float> _durations = new();

    private readonly ConcurrentQueue<IEffect> QueuedEffects = new();

    private readonly List<IEffect> _toRequeue = [];

    private bool IsProcessing =>
        GameActivePatches.IsGameActive == 1 && GUIManager.instance?.windowHidingUI == false;

    internal EffectIngress Ingress { get; private set; }
    internal EffectStateSnapshotter Snapshotter { get; private set; }

    public EffectManager(EffectManagerConfigurationRecord? configuration = null)
    {
        _configuration = configuration ??= EffectManagerConfigurationRecord.Default;

        Ingress = new EffectIngress(QueuedEffects);

        Snapshotter = new EffectStateSnapshotter(
            snapShotIntervalInMilliSeconds: _configuration.SnapshotIntervalInMilliSeconds,
            debugSnapshotLogOutput: _configuration.DebugSnapshotLogOutput,
            activatedEffects: _activatedEffects,
            activeEffects: _activeEffects,
            cooldowns: _cooldowns,
            durations: _durations,
            queuedEffects: QueuedEffects);
    }

    private bool IsEffectOnCooldown(IEffect effect)
    {
        return _cooldowns.TryGetValue(effect.Definition.Id, out var cooldown) && cooldown > 0;
    }

    private void ProcessEffect(IEffect effect)
    {
        if (!effect.Definition.ApplyEffect())
        {
            _toRequeue.Add(effect);
            return;
        }

        // Both
        _activatedEffects.Enqueue(effect);
        _cooldowns[effect.Definition.Id] = effect.Definition.CooldownInSeconds;

        //Timed
        if (effect is ITimedEffect timedEffect)
        {
            timedEffect.Activate();
            _activeEffects.TryAdd(effect.Guid, effect);
            _durations[timedEffect] = timedEffect.TimedDefinition.Duration;
        }
    }

    private void ProcessQueuedEffects()
    {
        while (QueuedEffects.TryDequeue(out var effect))
        {
            if (IsEffectOnCooldown(effect))
            {
                _toRequeue.Add(effect);
                continue;
            }

            ProcessEffect(effect);
        }
    }

    private void ProcessActiveEffects(float delta)
    {
        foreach (var effectKVP in _activeEffects.ToList())
        {
            var effect = effectKVP.Value;
            if (effect is not ITimedEffect timedEffect)
            {
                continue;
            }

            _durations[timedEffect] -= delta;
            var remainingDuration = _durations[timedEffect];

            if (remainingDuration > 0f)
            {
                continue;
            }

            Plugin.Log.LogDebug($"Effect {timedEffect.TimedDefinition.Id} expired");
            var succeedRemove = timedEffect.TimedDefinition.EndEffect();

            if (!succeedRemove)
            {
                continue;
            }

            Plugin.Log.LogDebug($"Effect {timedEffect.TimedDefinition.Id} removed");
            _activeEffects.TryRemove(effect.Guid, out _);
            _durations.TryRemove(timedEffect, out _);
            timedEffect.DeActivate();
        }
    }

    public void Update(float deltaTime)
    {
        if (!IsProcessing)
        {
            return;
        }

        UpdateCooldowns(deltaTime);
        ProcessQueuedEffects();

        foreach (var effect in _toRequeue)
        {
            QueuedEffects.Enqueue(effect);
        }

        _toRequeue.Clear();
        ProcessActiveEffects(deltaTime);

        Snapshotter.Update();
    }

    // No need to update cooldowns if they are not active....
    // consider maintaining a sparse list of active cooldowns
    private void UpdateCooldowns(float deltaTime)
    {
        foreach (var key in _cooldowns.Keys.ToList())
        {
            var value = _cooldowns[key];
            var newValue = Math.Max(0f, value - deltaTime);
            if (newValue == 0)
            {
                _cooldowns.TryRemove(key, out _);
            }
            else
            {
                _cooldowns[key] = Math.Max(0f, value - deltaTime);
            }
        }
    }
}
