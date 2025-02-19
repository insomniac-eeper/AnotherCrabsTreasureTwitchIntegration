﻿/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using Immediate;
using Swan;
using Types;

public static class Definitions
{
    public static readonly ConcurrentDictionary<string, EffectDefinition> AllEffects;
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> s_overrides = new();

    public static bool GetEffectWithOverride(string id, out EffectDefinition effect)
    {
        if (!AllEffects.TryGetValue(id, out effect)) return false;

        if (!s_overrides.TryGetValue(id, out var overrideDict)) return true;

        if (overrideDict == null) return true;

        if (effect is TimedEffectDefinition timedEffect)
        {
            effect = new OverridenTimedEffect(
                timedEffect,
                nameOverride: overrideDict!.GetValueOrDefault("name"),
                descriptionOverride: overrideDict!.GetValueOrDefault("description"),
                cooldownInSecondsOverride: int.TryParse(overrideDict!.GetValueOrDefault("cooldownInSeconds"), out int cooldownInSeconds) ? cooldownInSeconds : null,
                durationOverride: int.TryParse(overrideDict!.GetValueOrDefault("duration"), out int duration) ? duration : null
            );
        }
        else
        {
            effect = new OverridenEffect(
                effect,
                nameOverride: overrideDict!.GetValueOrDefault("name"),
                descriptionOverride: overrideDict!.GetValueOrDefault("description"),
                cooldownInSecondsOverride: int.TryParse(overrideDict!.GetValueOrDefault("cooldownInSeconds"), out int cooldownInSeconds) ? cooldownInSeconds : null
            );
        }
        return true;
    }

    static Definitions()
    {
        Plugin.Log.LogDebug($"Effect Definition static initializer.....");
        AllEffects = new ConcurrentDictionary<string, EffectDefinition>(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        !(
                            typeof(OverridenEffect).IsAssignableFrom(t) ||
                            typeof(OverridenTimedEffect).IsAssignableFrom(t) ||
                            t == typeof(ModifyKrillStat) ||
                            t == typeof(AddAfflictionToKrill)
                        ) &&
                        (
                            typeof(EffectDefinition).IsAssignableFrom(t) ||
                            typeof(TimedEffectDefinition).IsAssignableFrom(t)
                        ) &&
                        t.GetConstructor(Type.EmptyTypes) != null) // Ensure the type has a default constructor
            .Select(Activator.CreateInstance)
            .OfType<EffectDefinition>()
            .ToDictionary(e => e.Id, e => e));
    }
}
