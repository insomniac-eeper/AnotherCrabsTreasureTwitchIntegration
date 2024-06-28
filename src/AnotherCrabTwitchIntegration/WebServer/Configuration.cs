/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.WebServer;

public class Configuration
{
    // [General]
    public string BaseUrl = "http://127.0.0.1:12345/";
    public bool EnableWebSocketServer = true;
    public int SnapshotPollingInMilliseconds = 100;
}