/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Common.TwitchLibrary.Models;
using HarmonyLib;
using Modules;
using Modules.Effects;
using Modules.EnemySpawning;
using Modules.TwitchIntegration;
using Modules.TwitchIntegration.Patches.GUIModification;
using Modules.TwitchIntegration.Types;
using WebServer;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets shared logging instance.
    /// </summary>
    public static ManualLogSource Log { get; private set; }

    private static ConcurrentDictionary<string, IModule> _modules = new();

    // This is dodgy...
    public static IReadOnlyDictionary<string, IModule> Modules => new ReadOnlyDictionary<string, IModule>(_modules);

    public static SnapshotWebServer WebServer;


    private void Awake()
    {
        Log = Logger;

        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        var effectsModule = new EffectsModule();
        _modules.TryAdd(nameof(EffectsModule), effectsModule);
        effectsModule.Initialize();

        var twitchIntegrationModule = new TwitchIntegrationModule();
        _modules.TryAdd(nameof(TwitchIntegrationModule), twitchIntegrationModule);
        twitchIntegrationModule.Initialize(effectsModule.EffectIngress);
        twitchIntegrationModule.TwitchIntegration.OnConnectionStateChange += OnTIConnectionStateChange;

        WebServer = new SnapshotWebServer(effectsModule.EffectStateSnapshotter, effectsModule.EffectIngress, "http://127.0.0.1:12345/");
        WebServer.Start();

        var enemySpawningModule = new EnemySpawningModule();
        _modules.TryAdd(nameof(EnemySpawningModule), enemySpawningModule);
        //enemySpawningModule.Initialize();

        ApplyPatches();
    }

    private void ApplyPatches()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnTIConnectionStateChange(object sender, TwitchConnectionRecord connectionRecord)
    {
        Log.LogError($"Setting version label to {connectionRecord.AuthenticationState}");
        VersionLabelPatches.SetVersionPrepend(connectionRecord.AuthenticationState is AuthState.Authenticated
            ? $"Twitch Integration\nSTATE: AUTHENTICATED\nUser: {connectionRecord.Username}"
            : "Twitch Integration\nSTATE: DISCONNECTED");
    }

}