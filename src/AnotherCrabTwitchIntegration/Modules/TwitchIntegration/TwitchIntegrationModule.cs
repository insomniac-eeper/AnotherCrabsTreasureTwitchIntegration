/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration;

using System;
using Effects;
using UnityEngine;
using Object = UnityEngine.Object;

public class TwitchIntegrationModule
{
    public TwitchIntegration TwitchIntegration;

    public void Initialize(EffectIngress effectIngress)
    {
        try
        {
            var twitchIntegrationGameObject = new GameObject(nameof(TwitchIntegration));
            TwitchIntegration = twitchIntegrationGameObject.AddComponent<TwitchIntegration>();
            Object.DontDestroyOnLoad(twitchIntegrationGameObject);
            TwitchIntegration.Initialize(effectIngress);
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to create {nameof(Modules.TwitchIntegration.TwitchIntegration)}: {ex}");
        }
    }

    public bool IsInitialized { get; private set; }
}