/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using System.Collections.Concurrent;
using Types;

public class EffectIngress(ConcurrentQueue<IEffect> effectQueue)
{
    public void TryAddEffect(string effectId, string requester)
    {
        if (!Definitions.GetEffectWithOverride(effectId, out var effectToDo))
        {
            Plugin.Log.LogError($"Effect with the id {effectId} not found.");
        }

        switch (effectToDo)
        {
            case TimedEffectDefinition timedEffectDefinition:
            {
                var nEffect = new TimedEffect(timedEffectDefinition, requester,
                    DateTimeOffset.Now.ToUnixTimeSeconds());
                effectQueue.Enqueue(nEffect);
                break;
            }
            case EffectDefinition:
            {
                var nEffect = new Effect(effectToDo, requester);
                effectQueue.Enqueue(nEffect);
                break;
            }
        }
    }
}