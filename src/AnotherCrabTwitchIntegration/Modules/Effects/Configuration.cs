/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System.Collections.Concurrent;
using BepInEx.Configuration;

public class Configuration
{
    // [General]
    public ConfigEntry<bool> StackingEnabled;

    // [SnapShot]
    public ConfigEntry<int> EffectSnapShotIntervalInMilliSeconds;
    public ConfigEntry<bool> DebugSnapShotLog;

    // [Effects]
    // Generate on Load
    public readonly ConcurrentDictionary<string, ConfigEntry<bool>> EffectsEnabled = new();
    public readonly ConcurrentDictionary<string, ConfigEntry<string>> EffectsActivationId = new();
    public readonly ConcurrentDictionary<string, ConfigEntry<int>> EffectsCooldown = new();

    public void BindToConfig(ConfigFile? configFile)
    {
        if (configFile == null)
        {
            return;
        }

        StackingEnabled = configFile.Bind("Effects", nameof(StackingEnabled), true,
            "If true, effects will be able stack on top of each other.");
        EffectSnapShotIntervalInMilliSeconds = configFile.Bind("Effects", nameof(EffectSnapShotIntervalInMilliSeconds), 1,
            "The interval in milliseconds between snapshots of the effect state.");

        DebugSnapShotLog = configFile.Bind("Effects", nameof(DebugSnapShotLog), false,
            "If true, debug logs will be printed for snapshots.");

        foreach (var effect in Definitions.AllEffects)
        {
            EffectsEnabled.TryAdd(effect.Key,
                configFile.Bind($"Effects.Definitions.{effect.Key}", $"{effect.Key}.Enabled", true,
                    "If true, this effect will be enabled."));
            EffectsActivationId.TryAdd(effect.Key,
                configFile.Bind($"Effects.Definitions.{effect.Key}", $"{effect.Key}.ActivationId", effect.Value.Id,
                    "The activation id for this effect."));
            EffectsCooldown.TryAdd(effect.Key,
                configFile.Bind($"Effects.Definitions.{effect.Key}", $"{effect.Key}.Cooldown",
                    effect.Value.CooldownInSeconds, "The cooldown in seconds for this effect."));
        }
    }
}
