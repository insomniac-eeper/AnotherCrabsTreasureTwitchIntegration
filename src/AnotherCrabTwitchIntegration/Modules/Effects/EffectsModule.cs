/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects;

using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = UnityEngine.Object;

public class EffectsModule
{

    public EffectManager EffectManager;
    public EffectIngress EffectIngress;
    public EffectStateSnapshotter EffectStateSnapshotter;

    private Configuration _configuration;
    public void Initialize(GameObject target  = null, ConfigFile config = null)
    {
        try
        {
            var targetGameObject = target == null ? new GameObject(nameof(EffectManager)) : target;
            EffectManager = new EffectManager();
            EffectIngress = EffectManager.Ingress;
            EffectStateSnapshotter = EffectManager.Snapshotter;

            _configuration = new Configuration();
            _configuration.BindToConfig(config);

            _configuration.DebugSnapShotLog.SettingChanged += (sender, args) =>
            {
                EffectStateSnapshotter.DebugSnapshotLogOutput = _configuration.DebugSnapShotLog.Value;
            };
            _configuration.EffectSnapShotIntervalInMilliSeconds.SettingChanged += (sender, args) =>
            {
                EffectStateSnapshotter.EffectSnapShotIntervalInMilliSeconds = _configuration.EffectSnapShotIntervalInMilliSeconds.Value;
            };

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
}