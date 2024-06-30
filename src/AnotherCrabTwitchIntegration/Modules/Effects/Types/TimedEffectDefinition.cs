/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public abstract class TimedEffectDefinition : EffectDefinition
{
    public int Duration { get; }
    public event EffectAction? OnEndEffect;

    protected TimedEffectDefinition(
        string id,
        string name,
        string description,
        int cooldownInSeconds = 10,
        int duration = 10,
        EffectAction? startEffect = null,
        EffectAction? endEffect = null) : base(id, name, description, cooldownInSeconds, startEffect)
    {
        Duration = duration;
        if (endEffect != null)
        {
            OnEndEffect += endEffect;
        }
    }

    public bool EndEffect()
    {
        return OnEndEffect?.Invoke() ?? true;
    }

}
