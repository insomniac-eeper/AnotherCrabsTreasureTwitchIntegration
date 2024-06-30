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
    internal readonly ConcurrentDictionary<string, GameObject> _cachedEnemies = new();
    internal readonly ConcurrentDictionary<string, GameObject> _cachedBosses = new();

    public GameObject SpawnGo(GameObject go, Vector3 position = default, bool doActivate = false)
    {
        var spawnLoc = position == default ? Player.singlePlayer.transform.position : position;

        var newGo = Object.Instantiate(go, spawnLoc, Quaternion.identity);
        if (doActivate)
        {
            EnemyHelpers.SetAllChildrenProblematicComponents(newGo, true);
        }

        return newGo;
    }

    public bool SpawnBossEnemy<T>(
        GameObject? enemyOrig = null,
        string name = "",
        Action<T>? additionalSetupAction = null,
        Action<T>? postSpawnAction = null,
        Vector3? spawnPoint = default) where T : Enemy
    {
        if (!enemyOrig)
        {
            enemyOrig = _cachedBosses
                .Where(x => x.Value.name.Contains(name))
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        if (enemyOrig == null)
        {
            Plugin.Log.LogError($"Unable to find enemy boss with component {nameof(T)} and name: {name}");
            return false;
        }

        spawnPoint ??= GetRandomOffsetPosition(Player.singlePlayer.transform.position);

        var newEnemy = SpawnGo(enemyOrig, (Vector3)spawnPoint, doActivate: false);
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

        if (enemyView == null)
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

        // Not all bosses have this component, so it is not an error if not found
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
        bossComponent.skipCutscene = true;
        bossComponent.skipHealingPlayerOnDeath = true;
        bossComponent.spawnDropsAfterChatter = false;
        bossComponent.hasAchievement = false;

        genericComponent.enabled = true;
        genericComponent.dropOnDeath = [];
        genericComponent.dropHatWhenDowned = false;
        genericComponent.dropData = [];
        genericComponent.overrideUmamiDrop = true;
        genericComponent.overrideUmamiValue = 0;
        genericComponent.acquireOnDeath = [];

        postSpawnAction?.Invoke(genericComponent);
        return true;
    }

    //TODO: Create a function which looks for a "valid" spawn point meaning it is not in a wall or under the ground or over a drop...
    private Vector3 GetRandomOffsetPosition(Vector3 t)
    {
        var randomOffset = new Vector3(
            UnityEngine.Random.Range(-10f, 10f),
            UnityEngine.Random.Range(-0, 10f), // to avoid spawning under the ground
            UnityEngine.Random.Range(-10f, 10f));
        return new Vector3(t.x + randomOffset.x, t.y + randomOffset.y, t.z + randomOffset.z);
    }

    private void RemoveAchievementHelper(Component component)
    {
        var bossAchievementHelper = component.gameObject.GetComponent<BossAchievmentHelper>();
        if (bossAchievementHelper)
        {
            // TODO: Evaluate if we can just destroy the component instead of lobotomizing it...
            bossAchievementHelper.achievementID = string.Empty;
            bossAchievementHelper.saveState = null;
            bossAchievementHelper.enabled = false;
        }
        else
        {
            Plugin.Log.LogDebug($"Component {nameof(BossAchievmentHelper)} not found on {component.gameObject.name}");
        }
    }

    private Transform CreateNodesCenterAtRandomPositionCloseToPlayer()
    {
        var spawnPoint = GetRandomOffsetPosition(Player.singlePlayer.transform.position);
        var randomNodesCenter = new GameObject("NodesCenter");
        var transform = randomNodesCenter.GetComponent<Transform>();
        transform.position = spawnPoint;
        return transform;
    }

    public bool SpawnTopoda(GameObject? topodaOrig = null)
    {
        var spawnPoint = CreateNodesCenterAtRandomPositionCloseToPlayer();

        return SpawnBossEnemy<Topoda>(
            enemyOrig: topodaOrig,
            name:"Topoda",
            spawnPoint: spawnPoint.position,
            additionalSetupAction: (topodaComponent) => topodaComponent.nodesCenter = spawnPoint,
            postSpawnAction:(topodaComponent) => topodaComponent.TriggerAggro());
    }

    public bool SpawnHeikea(GameObject? heikeaOrig = null)
    {
        return SpawnBossEnemy<Heikea>(
            enemyOrig: heikeaOrig,
            name:"Heikea");
    }

    public bool SpawnInkerton(GameObject? inkertonOrig = null)
    {
        return SpawnBossEnemy<Inkerton>(
            enemyOrig: inkertonOrig,
            name:"Inkerton");
    }

    // Currently bugged. Will take damage from single hit event infinitely. Need to see if knots are to blame.
    public bool SpawnConsortium(GameObject? consortiumOrig = null)
    {
        return SpawnBossEnemy<Consortium>(
            enemyOrig: consortiumOrig,
            name:"Consortium");
    }

    public bool SpawnLichenthrope(GameObject? lichenthropeOrig = null)
    {
        return SpawnBossEnemy<FrogFish>(
            enemyOrig: lichenthropeOrig,
            name:"Lichenthrope");
    }

    public bool SpawnPagurus(GameObject? pagurusOrig = null)
    {
        return SpawnBossEnemy<Pagurus>(
            enemyOrig: pagurusOrig,
            name:"Pagurus");
    }

    public bool SpawnBruiserBoss(GameObject? bruiserBossOrig = null)
    {
        return SpawnBossEnemy<BottleBruiser>(
            enemyOrig: bruiserBossOrig,
            name:"Bruiser_Boss Variant");
    }

    // Looks like we are still dropping a fruit sticker on death... And getting umami from it....
    public bool SpawnBruiserGrove(GameObject? bruiserGroveOrig = null)
    {
        return SpawnBossEnemy<Bruiser>(
            enemyOrig: bruiserGroveOrig,
            name:"BruiserGrove",
            additionalSetupAction: RemoveAchievementHelper);
    }

    // TODO: One of the components requires a patrol route. Either remove the component or add a patrol route.
    public bool SpawnExecutioner(GameObject? executionerOrig = null)
    {
        return SpawnBossEnemy<Lobster>(
            enemyOrig: executionerOrig,
            name:"Executioner Boss Variant",
            additionalSetupAction: RemoveAchievementHelper);
    }

    public bool SpawnBruiserScuttleport(GameObject? bruiserScuttleportOrig = null)
    {
        return SpawnBossEnemy<Bruiser>(
            enemyOrig: bruiserScuttleportOrig,
            name:"BruiserScuttleport",
            additionalSetupAction: RemoveAchievementHelper);
    }

    // TODO: Test interaction if boss dies and attempts to start phase 2.
    // TODO: Make sure the boss if facing player when spawned if possible
    // TODO: Overwrite Die to avoid achievement and phase 2
    public bool SpawnBleachedKing(GameObject? bleachedKingOrig = null)
    {
        return SpawnBossEnemy<BleachedKing>(
            enemyOrig: bleachedKingOrig,
            name:"BleachedKing",
            additionalSetupAction: RemoveAchievementHelper,
            postSpawnAction: king => king.Aggro());
    }

    // Must be activated after spawning
    public bool SpawnMoltedKing(GameObject? moltedKingOrig = null)
    {
        return SpawnBossEnemy<MoltedKing>(
            enemyOrig: moltedKingOrig,
            name:"MoltedKing",
            additionalSetupAction: RemoveAchievementHelper,
            postSpawnAction: (moltedKingComponent) => moltedKingComponent.ActivateMoltedKing());
    }

    // Looks like the boss doesn't have audio loaded correctly. For example event:/Roland/Roland_ShortSwing
    public bool SpawnRoland(GameObject? rolandOrig = null)
    {
        return SpawnBossEnemy<Roland>(
            enemyOrig: rolandOrig,
            postSpawnAction: (rolandComponent) => RolandDisableOtherWrenches(rolandComponent.gameObject),
            name:"Roland");
    }

    private static void RolandDisableOtherWrenches(GameObject targetObject)
    {
        if (targetObject.name.Contains("Allen"))
        {
            if (targetObject.name.Contains(".00"))
            {
                targetObject.SetActive(false);
            }
            return;
        }

        foreach (Transform child in targetObject.transform)
        {
            RolandDisableOtherWrenches(child.gameObject);
        }
    }

    public bool SpawnNephro(GameObject? nephroOrig = null)
    {
        return SpawnBossEnemy<Nephro>(
            enemyOrig: nephroOrig,
            name:"Nephro");
    }

    // Throws constantly on update position. Need to investigate.
    public bool SpawnVoltai(GameObject? voltaiOrig = null)
    {
        return SpawnBossEnemy<Voltai>(
            enemyOrig: voltaiOrig,
            name:"Voltai");
    }

}
