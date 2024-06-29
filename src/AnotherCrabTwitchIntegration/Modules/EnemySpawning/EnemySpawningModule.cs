/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System;
using BepInEx.Configuration;
using Effects;
using Patches;
using UnityEngine;
using Object = UnityEngine.Object;

public class EnemySpawningModule
{
    public readonly EnemySpawner Spawner = new();
    private Configuration _configuration = new();

    public void Initialize(ConfigFile config = null)
    {
        _configuration.BindToConfig(config);

        if (!_configuration.IsEnabled.Value)
        {
            return;
        }

        if (_configuration.AutoTrawlAtStart.Value)
        {
            StartScreenPatches.OnTitleLoad = OnTitleLoad;
        }

        var spawnTopodaDef = new SpawnTopoda(Spawner);
        var spawnHeikeaDef = new SpawnHeikea(Spawner);

        Modules.Effects.Definitions.AllEffects.TryAdd(spawnTopodaDef.Id, spawnTopodaDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnHeikeaDef.Id, spawnHeikeaDef);

    }

    private void OnTitleLoad()
    {
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
}