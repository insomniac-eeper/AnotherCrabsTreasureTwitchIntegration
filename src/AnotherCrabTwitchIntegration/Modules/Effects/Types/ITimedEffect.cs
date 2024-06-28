/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public interface ITimedEffect : IEffect
{
    public TimedEffectDefinition TimedDefinition { get; }

    public bool IsActive();

    public void DeActivate();
}