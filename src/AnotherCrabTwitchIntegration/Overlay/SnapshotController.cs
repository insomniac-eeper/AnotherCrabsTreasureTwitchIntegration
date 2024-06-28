/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Overlay;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Modules.Effects.Types;

public class SnapshotController(Func<EffectManagerStateSnapshotRecord> getSnapshot, int eventIntervalInMilliseconds = 100) : WebApiController
{
    [Route(HttpVerbs.Get, "/")]
    public async Task GetSnapshotStream()
    {
        HttpContext.Response.ContentType = "text/event-stream";
        using var writer = new StreamWriter(HttpContext.Response.OutputStream, Encoding.UTF8);

        while (true)
        {
            var snapshot = getSnapshot();
            var snapshotJson = JsonSerializer.Serialize(snapshot);
            await writer.WriteAsync($"data: {snapshotJson}\n\n");
            await writer.FlushAsync();

            await Task.Delay(eventIntervalInMilliseconds); // Wait for 1/10 second before sending the next event
        }
    }
}