/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = UnityEngine.Object;
using StartScreen = StartScreen;

public class EnemySpawningModule : IModule
{
    public readonly EnemySpawner Spawner = new();
    private Configuration _configuration = new();

    public void Initialize(ConfigFile configFile = null)
    {
        _configuration.BindToConfig(configFile);

        if (!_configuration.IsEnabled.Value)
        {
            return;
        }

        if (_configuration.AutoTrawlAtStart.Value)
        {
            On.StartScreen.Init += OnTitleLoad;
        }
    }

    private void OnTitleLoad(On.StartScreen.orig_Init orig, StartScreen self)
    {
        orig(self);
        if (IsInitialized)
        {
            return;
        }

        try
        {
            var enemyTrawler = new GameObject(nameof(SceneEnemyTrawler));
            var sceneTrawler = enemyTrawler.AddComponent<SceneEnemyTrawler>();

            sceneTrawler.Init(Spawner);
            Object.DontDestroyOnLoad(enemyTrawler);
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to create {nameof(SceneEnemyTrawler)}: {ex}");
            IsInitialized = false;
        }
    }

    public bool IsInitialized { get; private set; }
    public bool IsEnabled { get; private set; }
}