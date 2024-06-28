/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.WebServer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebApi;
using Modules.Effects;
using Modules.Effects.Types;
using Overlay;

public class SnapshotWebServer
{
    private readonly WebServer _server;
    private EffectManagerStateSnapshotRecord _currentSnapshot;
    private Task _serverTask;

    private EffectIngress _effectIngress;

    private readonly Dictionary<string, string> _staticFilesToServer;

    private const string MainOverlayId = "AnotherCrabTwitchIntegration.Overlay.webpage.index.html";

    private readonly Action<string> _onRequest;

    public SnapshotWebServer(EffectStateSnapshotter effectStateSnapshotter, EffectIngress effectIngress, string url)
    {
        _effectIngress = effectIngress;
        _staticFilesToServer = LoadAllStaticFilesFromResources();
        _staticFilesToServer[MainOverlayId] = PrepareOverlayIndex(_staticFilesToServer[MainOverlayId], url);

        _server = CreateWebServer(url);
        effectStateSnapshotter.OnSnapshot += OnSnapshot;
        _onRequest = OnRequest;
    }

    private string PrepareOverlayIndex(string overlayHtml, string url)
    {
        return overlayHtml.Replace(@"var baseUrl = 'http://127.0.0.1:12345';", @$"var baseUrl = '{url}';");
    }

    private Dictionary<string, string> LoadAllStaticFilesFromResources()
    {
        var assembly = Assembly.GetExecutingAssembly();
        List<string> resourceStrings = [MainOverlayId, "AnotherCrabTwitchIntegration.Overlay.webpage.anime.js"];

        Dictionary<string, string> resources = new();

        Plugin.Log.LogDebug($"AssemblyName: {Assembly.GetExecutingAssembly().GetName().Name}");
        foreach (var resourceName in resourceStrings)
        {
            try
            {
                var resourceInfo = assembly
                    .GetManifestResourceInfo(resourceName);
                Plugin.Log.LogDebug($"Resource: {resourceInfo?.FileName}");

                var resource = assembly.GetManifestResourceStream(resourceName);

                if (resource != null)
                {
                    Plugin.Log.LogDebug($"The Length: {resource.Length}");
                }
                else
                {
                    Plugin.Log.LogDebug("Resource is null");
                    continue;
                }

                using var streamReader = new StreamReader(resource);
                resources.Add(resourceName, streamReader.ReadToEnd());
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug($"Error getting resource {resourceName}: {ex}");
            }
        }

        return resources;
    }

    private WebServer CreateWebServer(string url, int eventIntervalInSeconds = 100)
    {
        var server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
            .WithWebApi("/snapshot",
                m => m.WithController(() => new SnapshotController(() => _currentSnapshot, eventIntervalInSeconds)))
            .WithModule(new EffectIngressWebSocket("/ws", true, (msg) => _onRequest(msg)))
            .WithWebApi("/",
                m => m.WithController(() =>
                    new ResourceFileServeModule.StaticResourceModule("/", _staticFilesToServer[MainOverlayId])));

        return server;
    }

    public void Start()
    {
        _serverTask = Task.Run(() => _server.RunAsync().ConfigureAwait(false));
    }

    public async Task Stop()
    {
        await _serverTask;
    }

    private const string RequestId = "WS";

    private void OnRequest(string msg)
    {
        _effectIngress.TryAddEffect(msg, RequestId);
    }

    private void OnSnapshot(EffectManagerStateSnapshotRecord snapshot)
    {
        _currentSnapshot = snapshot;
    }
}