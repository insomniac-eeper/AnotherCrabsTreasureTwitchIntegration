/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System.Collections.Concurrent;
using System.Linq;
using Extensions;
using UnityEngine;

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
            EnemyHelpers.SetAllChildrenProceduralLegControllers(newGO, true);
        }
        return newGO;
    }

    public void SpawnTopoda(GameObject topodaOrig =  null)
    {
        if (topodaOrig == null)
        {
            topodaOrig = CachedBosses
                .Where(x => x.Value.name.Contains("Topoda"))
                .Select( x =>x.Value)
                .FirstOrDefault();
        }

        var spawnPoint = Player.singlePlayer.transform;

        var newTopoda = SpawnGO(topodaOrig, spawnPoint.position, doActivate: false);
        var TopodaComponent = newTopoda.GetComponent<Topoda>();
        TopodaComponent._hasTriedViewGet = false;
        TopodaComponent._hasTriedEnemyViewGet = true;

        //newTopoda.GetComponent<Enemy>().enabled = true;
        var TopodaView = newTopoda.GetChildWithName("View");
        var TopodaViewEnemyView = TopodaView.GetComponent<EnemyView>();
        TopodaComponent._enemyView = TopodaViewEnemyView;
        TopodaComponent.nodesCenter = spawnPoint;

        EnemyHelpers.SetAllChildrenProceduralLegControllers(newTopoda, true);
        newTopoda.GetComponent<Boss>().enabled = true;
        TopodaComponent.enabled = true;
    }

    private void SpawnHeika(GameObject heikeaOrig = null)
    {
        if (heikeaOrig == null)
        {
            heikeaOrig = CachedBosses
                .Where(x => x.Value.name.Contains("Heikea"))
                .Select( x =>x.Value)
                .FirstOrDefault();
        }

        var spawnPoint = Player.singlePlayer.transform;

        var newHeikea = SpawnGO(heikeaOrig, spawnPoint.position, doActivate: false);
        var heikeaComponent = newHeikea.GetComponent<Heikea>();
        heikeaComponent._hasTriedViewGet = false;
        heikeaComponent._hasTriedEnemyViewGet = true;

        //newTopoda.GetComponent<Enemy>().enabled = true;
        var heikeaView = newHeikea.GetChildWithName("View");
        var heikeaViewEnemyView = heikeaView.GetComponent<EnemyView>();
        heikeaComponent._enemyView = heikeaViewEnemyView;
        //heikeaComponent.

        EnemyHelpers.SetAllChildrenProceduralLegControllers(newHeikea, true);
        newHeikea.GetComponent<Boss>().enabled = true;
        heikeaComponent.enabled = true;
    }

}