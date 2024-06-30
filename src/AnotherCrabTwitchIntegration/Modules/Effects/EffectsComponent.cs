/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using UnityEngine;

public class EffectsComponent : MonoBehaviour
{
    public EffectManager? EffectManager;
    public void Initialize(EffectManager? effectManager)
    {
        EffectManager = effectManager;
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        EffectManager?.Update(deltaTime);
    }
}
