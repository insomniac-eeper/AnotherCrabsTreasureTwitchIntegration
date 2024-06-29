/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using BepInEx.Configuration;

/// <summary>
/// Configuration options for the <see cref="WebServerModule"/>.
/// </summary>
public class Configuration
{
    // [General]

    public ConfigEntry<bool> WebServerEnabled;
    public ConfigEntry<string> BaseWebServerUrl;
    public ConfigEntry<bool> EnableWebSocketServer;
    public ConfigEntry<bool> EnableOverlay;
    public ConfigEntry<int> SnapshotPollingInMilliseconds;
    public ConfigEntry<bool> AddCorsHeaders;

    public void BindToConfig(ConfigFile configFile)
    {
        if (configFile == null)
        {
            return;
        }

        WebServerEnabled = configFile?.Bind("WebServer", nameof(WebServerEnabled), true,
            "If true, the web server will be enabled.");

        BaseWebServerUrl = configFile?.Bind("WebServer", nameof(BaseWebServerUrl), "http://127.0.0.1:12345/",
            "The base URL for the web server.");
        EnableWebSocketServer = configFile?.Bind("WebServer", nameof(EnableWebSocketServer), true,
            "If true, the web server will enable the WebSocket server.");
        EnableOverlay = configFile?.Bind("WebServer", nameof(EnableOverlay), true,
            "If true, the web server will enable the overlay.");
        SnapshotPollingInMilliseconds = configFile?.Bind("WebServer", nameof(SnapshotPollingInMilliseconds), 100,
            "The polling rate in milliseconds between snapshots of the effect state. Ensure this is equal to your snapshotting interval rate.");
        AddCorsHeaders = configFile?.Bind("WebServer", nameof(AddCorsHeaders), false, "Add CORS headers to the web server for debug purposes.");
    }
}