/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using BepInEx.Configuration;
using Effects;

public class WebServerModule
{
    public bool IsInitialized { get; private set; }

    private Configuration _configuration;
    private ACTWebServer _webServer;

    public void Initialize(EffectStateSnapshotter stateSnapshotter = null, EffectIngress effectIngress = null, ConfigFile config = null)
    {
        _configuration = new Configuration();
        _configuration.BindToConfig(config);

        if (_configuration.WebServerEnabled.Value == false)
        {
            return;
        }

        _webServer = new ACTWebServer(stateSnapshotter,
            effectIngress: effectIngress,
            url: _configuration.BaseWebServerUrl.Value,
            eventIntervalInMilliseconds: _configuration.SnapshotPollingInMilliseconds.Value,
            addCors: _configuration.AddCorsHeaders.Value
            );

        _webServer.Start();

        IsInitialized = true;
    }
}