/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using System;
using System.Threading.Tasks;
using EmbedIO.WebSockets;

/// <summary>
/// Websocket module for handling incoming messages and forwarding them to the webserver via callback.
/// </summary>
/// <param name="urlPath">Path on which to server the websocket server.</param>
/// <param name="enableConnectionWatchdog">Whether contexts with closed connections should be purged every 30 seconds.</param>
/// <param name="onRequestCb">Passes incoming message back to webserver.</param>
public class EffectIngressWebSocket(string urlPath, bool enableConnectionWatchdog, Action<string> onRequestCb)
    : WebSocketModule(urlPath, enableConnectionWatchdog)
{
    protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
    {
        string msg = Encoding.GetString(buffer);
        Plugin.Log.LogDebug($"Received message: {msg}");
        try
        {
            onRequestCb(msg);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error processing message: {e.Message}");
        }

        return Task.CompletedTask;
    }
}
