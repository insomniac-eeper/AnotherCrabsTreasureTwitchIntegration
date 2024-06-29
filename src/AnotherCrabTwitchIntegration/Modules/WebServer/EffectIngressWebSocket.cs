/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using System;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using JetBrains.Annotations;

/// <summary>
/// Websocket module for handling incoming messages and forwarding them to the webserver via callback.
/// </summary>
/// <param name="urlPath">Path on which to server the websocket server.</param>
/// <param name="enableConnectionWatchdog">Whether contexts with closed connections should be purged every 30 seconds.</param>
/// <param name="OnRequestCB">Passes incoming message back to webserver.</param>
public class EffectIngressWebSocket([NotNull] string urlPath, bool enableConnectionWatchdog, Action<string> OnRequestCB)
    : WebSocketModule(urlPath, enableConnectionWatchdog)
{
    protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
    {
        var msg = Encoding.GetString(buffer);
        Plugin.Log.LogDebug($"Received message: {msg}");
        try
        {
            OnRequestCB(msg);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error processing message: {e.Message}");
        }

        return Task.CompletedTask;
    }
}