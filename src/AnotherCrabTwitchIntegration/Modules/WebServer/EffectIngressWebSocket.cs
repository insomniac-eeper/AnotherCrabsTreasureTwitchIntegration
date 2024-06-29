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