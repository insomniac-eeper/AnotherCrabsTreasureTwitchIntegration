/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using BepInEx.Configuration;
using Effects;

/// <summary>
/// Contains the webserver and is responsible for initializing it appropriately based on config and exposing it.
/// </summary>
public class WebServerModule
{
    /// <summary>
    /// Whether the web server has been initialized successfully.
    /// </summary>
    public bool IsInitialized { get; private set; }

    private Configuration _configuration;
    private ACTWebServer _webServer;

    /// <summary>
    /// Configures and spins up the web server.
    /// </summary>
    /// <param name="stateSnapshotter">Source of state snapshots.</param>
    /// <param name="effectIngress">Allows the addition of effects.</param>
    /// <param name="config">Defines initialization behavior. Must be bound.</param>
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