﻿/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnemyTrawler : MonoBehaviour
{
    private static bool s_hasLoaded;
    private static readonly ConcurrentDictionary<string, bool> s_scenesUnloaded = new();
    private static EnemySpawner? s_enemySpawner;

    public void Init(EnemySpawner enemySpawner)
    {
        s_enemySpawner = enemySpawner;
    }

    public void Start()
    {
        if (s_hasLoaded)
        {
            return;
        }

        foreach (var scene in Definitions.ScenesWithEnemies)
        {
            StartCoroutine(LoadYourAsyncScene(scene));
        }

        s_hasLoaded = true;
    }

    public void UnloadScene(string sceneName)
    {
        var unloadAsync = SceneManager.UnloadSceneAsync(sceneName);

        if (unloadAsync == null)
        {
            Plugin.Log.LogError($"Failed to unload scene {sceneName}");
            return;
        }

        unloadAsync.completed += _ =>
        {
            Plugin.Log.LogDebug($"Scene {sceneName} unloaded.");
            s_scenesUnloaded.TryUpdate(sceneName, true, false);

            if (s_scenesUnloaded.ToList().All(x => x.Value))
            {
                Plugin.Log.LogDebug("All scenes unloaded.");
                SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
            }
        };
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        s_scenesUnloaded.TryAdd(sceneName, false);
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (asyncLoad == null)
        {
            Plugin.Log.LogError($"Failed to load scene {sceneName}");
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {
            Plugin.Log.LogInfo($"Loading {sceneName} | {asyncLoad.progress * 100}%");
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        asyncLoad.completed += _ =>
        {
            Plugin.Log.LogInfo($"Scene {sceneName} loaded.");
            var rootGOs = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            CacheEnemyAndBossGameObjects(rootGOs);
            UnloadScene(sceneName: sceneName);
        };
    }

    private record struct SceneGameObjectAggregate(
        List<GameObject> Enemies,
        List<GameObject> Bosses,
        List<GameObject> Remaining);

    private static SceneGameObjectAggregate AggregateSceneGameObjects(GameObject[] rootGOs)
    {
        var aggregationSeed = new SceneGameObjectAggregate([], [], []);
        var aggregatedGameObjects = rootGOs.Aggregate(
            aggregationSeed,
            (result, go) =>
            {
                if (go.name.StartsWith("Enemies"))
                {
                    result.Enemies.Add(go);
                }
                else if (go.name.StartsWith("Bosses"))
                {
                    result.Bosses.Add(go);
                }
                else
                {
                    result.Remaining.Add(go);
                }

                return result;
            });

        return aggregatedGameObjects;
    }

    private static bool CacheEnemyAndBossGameObjects(GameObject[] rootGOs)
    {
        var (enemiesContainer, bossesContainer, remainingGOs) = AggregateSceneGameObjects(rootGOs);

        // First let's destroy all the irrelevant GameObjects to save memory
        foreach (var destGo in remainingGOs)
        {
            Destroy(destGo);
        }

        if (enemiesContainer.Count < 1 && bossesContainer.Count < 1)
        {
            return false;
        }

        TraverseContainersForMatchingObjects(enemiesContainer, typeof(SaveStateKillableEntity), s_enemySpawner?.CachedEnemies);
        TraverseContainersForMatchingObjects(bossesContainer, typeof(Boss), s_enemySpawner?.CachedBosses);
        return true;
    }

    // TODO: Adapt this to go down hierarchy. Sometimes Containers have nested game objects with the componentTypeToCheck...
    private static void TraverseContainersForMatchingObjects(List<GameObject> containers, Type componentTypeToCheck, ConcurrentDictionary<string, GameObject> targetDictionary)
    {
        foreach (var container in containers)
        {
            foreach (object? child in container.transform)
            {
                var childGameObject = ((Transform) child).gameObject;

                if (childGameObject.GetComponent(componentTypeToCheck) == null)
                {
                    continue;
                }
                // We do this since we don't want the instantiated GameObject to spawn its IK holders and other things.
                EnemyHelpers.SetAllChildrenProblematicComponents(childGameObject, false);
                var newInstance = Instantiate(childGameObject);
                DontDestroyOnLoad(newInstance);
                EnemyHelpers.SetAllChildrenProblematicComponents(newInstance, false); // As a precaution but could maybe be removed
                targetDictionary.TryAdd(newInstance.name, newInstance);
            }
        }
    }
}
