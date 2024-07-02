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

    public void Initialize(ConfigFile? config = null)
    {
        _configuration.BindToConfig(config);

        if (!_configuration.IsEnabled.Value)
        {
            return;
        }

        if (_configuration.AutoTrawlAtStart.Value)
        {
            StartScreenPatches.s_onTitleLoad = OnTitleLoad;
        }

        var spawnTopodaDef = new SpawnTopoda(Spawner);
        var spawnHeikeaDef = new SpawnHeikea(Spawner);
        var spawnNephroDef = new SpawnNephro(Spawner);
        var spawnVoltaiDef = new SpawnVoltai(Spawner);
        var spawnFirthDef = new SpawnFirth(Spawner);
        var spawnPetrochDef = new SpawnPetroch(Spawner);
        var spawnInkertonDef = new SpawnInkerton(Spawner);
        var spawnConsortiumDef = new SpawnConsortium(Spawner);
        var spawnLichenthropeDef = new SpawnLichenthrope(Spawner);
        var spawnPagurusDef = new SpawnPagurus(Spawner);
        var spawnBruiserBossDef = new SpawnBruiserBoss(Spawner);
        var spawnBruiserGroveBossDef = new SpawnBruiserGroveBoss(Spawner);
        var spawnExecutionerDef = new SpawnExecutioner(Spawner);
        var spawnBruiserScuttleportDef = new SpawnBruiserScuttleport(Spawner);
        var spawnBleachedKingDef = new SpawnBleachedKing(Spawner);
        var spawnMoltedKingDef = new SpawnMoltedKing(Spawner);
        var spawnRolandDef = new SpawnRoland(Spawner);

        Modules.Effects.Definitions.AllEffects.TryAdd(spawnTopodaDef.Id, spawnTopodaDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnHeikeaDef.Id, spawnHeikeaDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnNephroDef.Id, spawnNephroDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnVoltaiDef.Id, spawnVoltaiDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnFirthDef.Id, spawnFirthDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnPetrochDef.Id, spawnPetrochDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnInkertonDef.Id, spawnInkertonDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnConsortiumDef.Id, spawnConsortiumDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnLichenthropeDef.Id, spawnLichenthropeDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnPagurusDef.Id, spawnPagurusDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnBruiserBossDef.Id, spawnBruiserBossDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnBruiserGroveBossDef.Id, spawnBruiserGroveBossDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnExecutionerDef.Id, spawnExecutionerDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnBruiserScuttleportDef.Id, spawnBruiserScuttleportDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnBleachedKingDef.Id, spawnBleachedKingDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnMoltedKingDef.Id, spawnMoltedKingDef);
        Modules.Effects.Definitions.AllEffects.TryAdd(spawnRolandDef.Id, spawnRolandDef);
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
