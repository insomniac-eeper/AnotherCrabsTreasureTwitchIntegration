/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

using System;

public class Effect(EffectDefinition definition, string requester) : IEffect
{
    public Guid Guid { get; } = Guid.NewGuid();
    public EffectDefinition Definition { get; } = definition;
    public string Requester { get; } = requester;
    public long RequestTimeStamp { get; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    public long ActivationTimeStamp { get; private set; }

    public void Activate()
    {
        ActivationTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}