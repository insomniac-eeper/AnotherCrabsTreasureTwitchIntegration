/*
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
    private static bool _hasLoaded;
    private static readonly ConcurrentDictionary<string, bool> ScenesUnloaded = new();
    private static EnemySpawner _enemySpawner;

    public void Init(EnemySpawner enemySpawner)
    {
        _enemySpawner = enemySpawner;
    }

    public void Start()
    {
        if (_hasLoaded)
        {
            return;
        }

        foreach (var scene in Definitions.ScenesWithEnemies)
        {
            StartCoroutine(LoadYourAsyncScene(scene));
        }

        _hasLoaded = true;
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
            Plugin.Log.LogError($"Scene {sceneName} unloaded.");
            ScenesUnloaded.TryUpdate(sceneName, true, false);

            if (ScenesUnloaded.ToList().All(x => x.Value))
            {
                Plugin.Log.LogError("All scenes unloaded.");
                SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
            }
        };
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        ScenesUnloaded.TryAdd(sceneName, false);
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
            Plugin.Log.LogError($"Loading {sceneName} | {asyncLoad.progress * 100}%");
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        asyncLoad.completed += _ =>
        {
            Plugin.Log.LogError($"Scene {sceneName} loaded.");
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

        TraverseContainersForMatchingObjects(enemiesContainer, typeof(SaveStateKillableEntity), _enemySpawner?.CachedEnemies);
        TraverseContainersForMatchingObjects(bossesContainer, typeof(Boss), _enemySpawner?.CachedBosses);
        return true;
    }

    private static void TraverseContainersForMatchingObjects(List<GameObject> containers, Type componentTypeToCheck, ConcurrentDictionary<string, GameObject> targetDictionary)
    {
        foreach (var container in containers)
        {
            foreach (var child in container.transform)
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