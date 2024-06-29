/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Effects;
using Effects.Types;
using EmbedIO;
using EmbedIO.WebApi;
using Overlay;

/// <summary>
/// WebServer to support the overlay and websocket server.
/// </summary>
public class ACTWebServer
{
    private readonly WebServer _server;
    private EffectManagerStateSnapshotRecord _currentSnapshot;
    private Task _serverTask;

    private EffectIngress _effectIngress;
    private EffectStateSnapshotter _effectStateSnapshotter;

    private readonly Dictionary<string, string> _staticFilesToServer;

    private const string MainOverlayId = "AnotherCrabTwitchIntegration.Modules.WebServer.Overlay.webpage.index.html";
    private const string AnimeJSId = "AnotherCrabTwitchIntegration.Modules.WebServer.Overlay.webpage.anime.min.js";

    private readonly bool _overlayEnabled;
    private readonly bool _webSocketServerEnabled;
    private readonly bool _addCors;

    private readonly Action<string> _onRequest;

    /// <summary>
    /// Performs the necessary setup for the webserver.
    /// </summary>
    /// <param name="effectStateSnapshotter">Source of effect state snapshots.</param>
    /// <param name="enableOverlay">Whether to enable overlay functionality, which consumes state snapshots.</param>
    /// <param name="effectIngress">Sink to queue new effect requests, which are added via websocket server.</param>
    /// <param name="enableWebSocketServer">Whether to enable the websocket server.</param>
    /// <param name="url">Root URL on which to host the webserver.</param>
    /// <param name="eventIntervalInMilliseconds">Polling rate for the overlay to check for new snapshot updates.</param>
    /// <param name="addCors">Add Cross-Origin-Resource-Sharing headers to test external html pages.</param>
    public ACTWebServer(
        EffectStateSnapshotter? effectStateSnapshotter = null,
        bool enableOverlay = true,
        EffectIngress? effectIngress = null,
        bool enableWebSocketServer = true,
        string url = "http://127.0.0.1:12345",
        int eventIntervalInMilliseconds = 100,
        bool addCors = false)
    {
        _overlayEnabled = enableOverlay;
        _webSocketServerEnabled = enableWebSocketServer;
        _addCors = addCors;

        _effectIngress = effectIngress;
        _effectStateSnapshotter = effectStateSnapshotter;

        if (_overlayEnabled && _effectStateSnapshotter != null)
        {
            _staticFilesToServer = LoadAllStaticFilesFromResources();
            _staticFilesToServer[MainOverlayId] = PrepareOverlayIndex(_staticFilesToServer[MainOverlayId], url);

            _addCors = addCors;
        }

        _server = CreateWebServer(url, eventIntervalInMilliseconds);

        if (_overlayEnabled && _effectStateSnapshotter != null)
        {
            _effectStateSnapshotter.OnSnapshot += OnSnapshot;
            _onRequest = OnRequest;
        }
    }

    private string PrepareOverlayIndex(string overlayHtml, string url)
    {
        return overlayHtml.Replace(@"http://127.0.0.1:12345", @$"{url}");
    }

    private Dictionary<string, string> LoadAllStaticFilesFromResources()
    {
        var assembly = Assembly.GetExecutingAssembly();
        List<string> resourceStrings = [MainOverlayId, AnimeJSId];

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

    private WebServer CreateWebServer(string url, int eventIntervalInMilliseconds = 100)
    {;
        var server = new WebServer(o =>  o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO));

        if (_addCors)
        {
            server = server.WithCors();
        }

        if (_overlayEnabled && _effectStateSnapshotter != null)
        {
            server = server
                .WithWebApi("/snapshot",
                    m => m.WithController(() => new SnapshotController(() => _currentSnapshot, eventIntervalInMilliseconds)))
                .WithModule(new FileStringModule(() => _staticFilesToServer[MainOverlayId], "/overlay", "text/html"))
                .WithModule(new FileStringModule(() =>_staticFilesToServer[AnimeJSId], "/dist/anime.min.js", "application/javascript"));
        }

        if (_webSocketServerEnabled && _effectIngress != null)
        {
            server = server
                .WithModule(new EffectIngressWebSocket("/ws", true, msg => _onRequest(msg)));
        }

        return server;
    }

    /// <summary>
    /// Start the web server.
    /// </summary>
    public void Start()
    {
        _serverTask = Task.Run(() => _server.RunAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// "Stop" the web server.
    /// </summary>
    /// <remarks>
    /// This method isn't currently used, but is here for completeness.
    /// </remarks>
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
