/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

using System;

public interface IEffect
{
    public Guid Guid { get; }

    public EffectDefinition Definition { get; }
    public string Requester { get; }
    public long RequestTimeStamp { get; }

    public long ActivationTimeStamp { get; }

    public void Activate();
}