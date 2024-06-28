/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

using System;

public class TimedEffect(TimedEffectDefinition timedDefinition, string requester, long requestTimeStamp)
    : ITimedEffect
{
    public Guid Guid { get; } = Guid.NewGuid();
    public EffectDefinition Definition { get; } = timedDefinition;
    public string Requester { get; } = requester;
    public long RequestTimeStamp { get; } = requestTimeStamp;
    public long ActivationTimeStamp { get; private set;}
    public TimedEffectDefinition TimedDefinition { get; } = timedDefinition;

    private bool _isActive;

    public bool IsActive()
    {
        return _isActive;
    }

    public void Activate()
    {
        ActivationTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        _isActive = true;
    }

    public void DeActivate()
    {
        _isActive = false;
    }

}