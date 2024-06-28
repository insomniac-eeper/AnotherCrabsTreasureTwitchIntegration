/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class EffectsModule : IModule
{

    public EffectManager EffectManager;
    public EffectIngress EffectIngress;
    public EffectStateSnapshotter EffectStateSnapshotter;
    public void Initialize(GameObject target  = null)
    {
        try
        {
            var targetGameObject = target == null ? new GameObject(nameof(EffectManager)) : target;
            EffectManager = new EffectManager();
            EffectIngress = EffectManager.Ingress;
            EffectStateSnapshotter = EffectManager.Snapshotter;
            var effectsComponent = targetGameObject.AddComponent<EffectsComponent>();
            effectsComponent.Initialize(EffectManager);
            Object.DontDestroyOnLoad(targetGameObject);
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to initialize EffectsModule: {ex}");
            IsInitialized = false;
        }
    }

    public bool IsInitialized { get; private set; }
    public bool IsEnabled { get; private set; }
}