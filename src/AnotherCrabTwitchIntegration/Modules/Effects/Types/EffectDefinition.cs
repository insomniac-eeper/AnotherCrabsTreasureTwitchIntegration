/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public delegate bool EffectAction();

public abstract class EffectDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public int CooldownInSeconds { get; }

    public event EffectAction OnStartEffect;

    public EffectDefinition(
        string id,
        string name,
        string description,
        int cooldownInSeconds,
        EffectAction? applyEffect = null)
    {
        Id = id;
        Name = name;
        Description = description;
        CooldownInSeconds = cooldownInSeconds;
        if (applyEffect != null)
        {
            OnStartEffect += applyEffect;
        }
    }

    public bool ApplyEffect()
    {
        return OnStartEffect?.Invoke() ?? true;
    }
}
