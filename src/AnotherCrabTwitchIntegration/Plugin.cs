/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration;

using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Common.TwitchLibrary.Models;
using HarmonyLib;

using Modules.Effects;
using Modules.EnemySpawning;
using Modules.TwitchIntegration;
using Modules.TwitchIntegration.Patches.GUIModification;
using Modules.TwitchIntegration.Types;
using Modules.WebServer;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets shared logging instance.
    /// </summary>
    public static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        var effectsModule = new EffectsModule();
        effectsModule.Initialize(config: Config);

        var twitchIntegrationModule = new TwitchIntegrationModule();
        twitchIntegrationModule.Initialize(effectsModule.EffectIngress);
        twitchIntegrationModule.TwitchIntegration.OnConnectionStateChange += OnTIConnectionStateChange;

        var enemySpawningModule = new EnemySpawningModule();
        enemySpawningModule.Initialize(config: Config);

        var webServerModule = new WebServerModule();
        webServerModule.Initialize(stateSnapshotter: effectsModule.EffectStateSnapshotter, effectIngress: effectsModule.EffectIngress, config: Config);

        ApplyPatches();
    }

    private void ApplyPatches()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnTIConnectionStateChange(object sender, TwitchConnectionRecord connectionRecord)
    {
        Log.LogDebug($"Setting version label to {connectionRecord.AuthenticationState}");
        VersionLabelPatches.SetVersionPrepend(connectionRecord.AuthenticationState is AuthState.Authenticated
            ? $"Twitch Integration\nSTATE: AUTHENTICATED\nUser: {connectionRecord.Username}"
            : "Twitch Integration\nSTATE: DISCONNECTED");
    }

}
