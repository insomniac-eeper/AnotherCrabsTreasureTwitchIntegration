/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System;
using System.Collections.Concurrent;
using System.Linq;
using Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

public class EnemySpawner
{
    internal readonly ConcurrentDictionary<string, GameObject> CachedEnemies = new();
    internal readonly ConcurrentDictionary<string, GameObject> CachedBosses = new();

    public GameObject SpawnGO(GameObject go, Vector3 position = default, bool doActivate = false)
    {
        var spawnLoc = position == default ? Player.singlePlayer.transform.position : position;

        var newGO = GameObject.Instantiate(go, spawnLoc, Quaternion.identity);
        if (doActivate)
        {
            EnemyHelpers.SetAllChildrenProblematicComponents(newGO, true);
        }

        return newGO;
    }

    public bool SpawnBossEnemy<T>(
        GameObject enemyOrig = null,
        string name = "",
        Action<T> additionalSetupAction = null,
        Transform spawnPoint = default) where T : Enemy
    {
        if (!enemyOrig)
        {
            enemyOrig = CachedBosses
                .Where(x => x.Value.name.Contains(name))
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        if (!enemyOrig)
        {
            Plugin.Log.LogError($"Unable to find enemy boss with component {nameof(T)} and name: {name}");
            return false;
        }

        if (spawnPoint == default)
        {
            spawnPoint = Player.singlePlayer.transform;
        }

        var newEnemy = SpawnGO(enemyOrig, spawnPoint.position, doActivate: false);
        newEnemy.AddComponent<CustomSpawn>();

        var genericComponent = newEnemy.GetComponent<T>();

        if (!genericComponent)
        {
            Plugin.Log.LogError($"Unable to get component {nameof(T)} from enemy boss with name: {name}");
            Object.Destroy(newEnemy);
            return false;
        }

        genericComponent._hasTriedViewGet = false;
        genericComponent._hasTriedEnemyViewGet = true;

        var enemyView = newEnemy.GetChildWithName("View");

        if (!enemyView)
        {
            Plugin.Log.LogError($"Unable to get child GameObject View from enemy boss with name: {name}");
            Object.Destroy(newEnemy);
            return false;
        }

        var enemyViewEnemyView = enemyView.GetComponent<EnemyView>();
        if (!enemyViewEnemyView)
        {
            Plugin.Log.LogError($"Unable to get Component EnemyView from child GameObject View from enemy boss with name: {name}");
            Object.Destroy(newEnemy);
            return false;
        }
        genericComponent._enemyView = enemyViewEnemyView;

        additionalSetupAction?.Invoke(genericComponent);

        var killableEntityComponent = newEnemy.GetComponent<SaveStateKillableEntity>();

        // Not all bosses have this component so it is not an error if not found
        if (!killableEntityComponent)
        {
            Plugin.Log.LogDebug($"Unable to get Component SaveStateKillableEntity from enemy boss with name: {name}");
        }
        else
        {
            killableEntityComponent.killPermanently = false;
        }

        EnemyHelpers.SetAllChildrenProblematicComponents(newEnemy, true);

        var bossComponent = newEnemy.GetComponent<Boss>();

        if (!bossComponent)
        {
            Plugin.Log.LogError($"Unable to get Component Boss from enemy boss with name: {name}");
            Object.Destroy(newEnemy);
            return false;
        }

        bossComponent.enabled = true;
        genericComponent.enabled = true;
        return true;
    }

    public bool SpawnTopoda(GameObject topodaOrig = null)
    {
        var spawnPoint = Player.singlePlayer.transform;

        return SpawnBossEnemy<Topoda>(
            enemyOrig: topodaOrig,
            name:"Topoda",
            spawnPoint: spawnPoint,
            additionalSetupAction:(topodaComponent) =>
        {
            topodaComponent.nodesCenter = spawnPoint;
        });
    }

    public bool SpawnHeikea(GameObject heikeaOrig = null)
    {
        return SpawnBossEnemy<Heikea>(
            enemyOrig: heikeaOrig,
            name:"Heikea");
    }

    public bool SpawnInkerton(GameObject inkertonOrig = null)
    {
        return SpawnBossEnemy<Inkerton>(
            enemyOrig: inkertonOrig,
            name:"Inkerton");
    }

    // Currently bugged. Will take damage from single hit event infinitely. Need to see if knots are to blame.
    public bool SpawnConsortium(GameObject consortiumOrig = null)
    {
        return SpawnBossEnemy<Consortium>(
            enemyOrig: consortiumOrig,
            name:"Consortium");
    }

    public bool SpawnLichenthrope(GameObject lichenthropeOrig = null)
    {
        return SpawnBossEnemy<FrogFish>(
            enemyOrig: lichenthropeOrig,
            name:"Lichenthrope");
    }

    public bool SpawnPagurus(GameObject pagurusOrig = null)
    {
        return SpawnBossEnemy<Pagurus>(
            enemyOrig: pagurusOrig,
            name:"Pagurus");
    }

    public bool SpawnBruiserBoss(GameObject bruiserBossOrig = null)
    {
        return SpawnBossEnemy<BottleBruiser>(
            enemyOrig: bruiserBossOrig,
            name:"Bruiser_Boss Variant");
    }

    public bool SpawnBruiserGrove(GameObject bruiserGroveOrig = null)
    {
        return SpawnBossEnemy<Bruiser>(
            enemyOrig: bruiserGroveOrig,
            name:"BruiserGrove",
            additionalSetupAction: (bruiserComponent) =>
            {
                var achievementHelper = bruiserComponent.gameObject.GetComponent<BossAchievmentHelper>();
                achievementHelper.achievementID = null;
                achievementHelper.saveState = null;
                achievementHelper.enabled = false;
            });
    }

    public bool SpawnExecutioner(GameObject executionerOrig = null)
    {
        return SpawnBossEnemy<Lobster>(
            enemyOrig: executionerOrig,
            name:"Executioner Boss Variant",
            additionalSetupAction: (lobsterComponent) =>
            {
                var achievementHelper = lobsterComponent.gameObject.GetComponent<BossAchievmentHelper>();
                achievementHelper.achievementID = null;
                achievementHelper.saveState = null;
                achievementHelper.enabled = false;
            });
    }

    public bool SpawnBruiserScuttleport(GameObject bruiserScuttleportOrig = null)
    {
        return SpawnBossEnemy<Bruiser>(
            enemyOrig: bruiserScuttleportOrig,
            name:"BruiserScuttleport",
            additionalSetupAction: (bruiserComponent) =>
            {
                var achievementHelper = bruiserComponent.gameObject.GetComponent<BossAchievmentHelper>();
                achievementHelper.achievementID = null;
                achievementHelper.saveState = null;
                achievementHelper.enabled = false;
            });
    }

    public bool SpawnBleachedKing(GameObject bleachedKingOrig = null)
    {
        return SpawnBossEnemy<BleachedKing>(
            enemyOrig: bleachedKingOrig,
            name:"BleachedKing",
            additionalSetupAction: (bruiserComponent) =>
            {
                var achievementHelper = bruiserComponent.gameObject.GetComponent<BossAchievmentHelper>();
                achievementHelper.achievementID = null;
                achievementHelper.saveState = null;
                achievementHelper.enabled = false;
            });
    }

    public bool SpawnMoltedKing(GameObject moltedKingOrig = null)
    {
        return SpawnBossEnemy<MoltedKing>(
            enemyOrig: moltedKingOrig,
            name:"MoltedKing",
            additionalSetupAction: (bruiserComponent) =>
            {
                var achievementHelper = bruiserComponent.gameObject.GetComponent<BossAchievmentHelper>();
                achievementHelper.achievementID = null;
                achievementHelper.saveState = null;
                achievementHelper.enabled = false;
            });
    }

}