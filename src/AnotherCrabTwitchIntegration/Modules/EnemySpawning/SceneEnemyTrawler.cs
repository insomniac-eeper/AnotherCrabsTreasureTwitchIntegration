/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

using TMPro;

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
    private record struct SceneLoadProgress(float Progress, bool IsLoaded);

    private static bool s_hasLoaded;
    private static readonly ConcurrentDictionary<string, SceneLoadProgress> s_scenesUnloaded = new();

    private float _sceneLoadProgress = 0f;
    private TextMeshProUGUI? _progressText;



    private static EnemySpawner? s_enemySpawner;

    public void Init(EnemySpawner enemySpawner)
    {
        s_enemySpawner = enemySpawner;
    }

    private const float baseScreenWidth = 1920;
    private const float baseFontSize = 20;

    float CalculateFontSizeForScreen()
    {
        float screenWidth = Screen.width;
        float desiredFontSize = screenWidth / 20;
        return desiredFontSize;
    }

    public void Start()
    {
        if (s_hasLoaded)
        {
            return;
        }

        var mainMenu = GameObject.Find("MainMenu") ??
                       throw new Exception($"Unable to find MainMenu GameObject.");

            foreach (Transform mmChild in mainMenu.transform)
            {
                if (mmChild.gameObject.name.Contains("Version"))
                {
                    continue;
                }

                mmChild.gameObject.SetActive(false);
            }


        var sceneLoadProgressGo = new GameObject("SceneLoadProgressText") { transform = { parent = mainMenu.transform } };
        var gameObjectRectTransform = sceneLoadProgressGo.AddComponent<RectTransform>();

        gameObjectRectTransform.anchorMin = new Vector2(0f, 0.5f);
        gameObjectRectTransform.anchorMax = new Vector2(1f, 0.5f);
        gameObjectRectTransform.anchoredPosition = Vector2.zero;
        gameObjectRectTransform.sizeDelta = new Vector2(0, 100);

        // Set t
        this._progressText = sceneLoadProgressGo.AddComponent<TextMeshProUGUI>();
        this._progressText.alignment = TextAlignmentOptions.Center;
        this._progressText.enableWordWrapping = false;
        this._progressText.fontSize = CalculateFontSizeForScreen();

        this._progressText.text = "Loading Enemy Data.... 0%";

        foreach (string? scene in Definitions.s_scenesWithEnemies)
        {
            StartCoroutine(LoadYourAsyncScene(scene));
        }

        s_hasLoaded = true;
    }

    private readonly int _maxSceneCount = Definitions.s_scenesWithEnemies.Count;

    public void Update()
    {
        if (!this._progressText)
        {
            return;
        }

        float sceneCount = Math.Max(_maxSceneCount, s_scenesUnloaded.Count);

        _sceneLoadProgress = s_scenesUnloaded.Select(x => x.Value.IsLoaded
                                    ? 100f
                                    : x.Value.Progress)
                                .Aggregate(0f,
                                    (acc,
                                        x) => acc + x) /
                            _maxSceneCount;
        this._progressText!.text = $"Loading Enemy Data.... {_sceneLoadProgress:F2}%";
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
            if (s_scenesUnloaded.TryGetValue(sceneName, out var oldValue))
            {
                s_scenesUnloaded.TryUpdate(sceneName, oldValue with { IsLoaded = true}, oldValue);
            }

            if (s_scenesUnloaded.ToList().All(x => x.Value.IsLoaded))
            {
                Plugin.Log.LogDebug("All scenes unloaded.");
                SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
            }
        };
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        SceneLoadProgress status = new(0f, false);
        s_scenesUnloaded.TryAdd(sceneName, status);
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
            if (s_scenesUnloaded.TryGetValue(sceneName, out var oldValue))
            {
                s_scenesUnloaded.TryUpdate(sceneName, oldValue with { Progress = asyncLoad.progress * 100 }, oldValue);
            }

            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        asyncLoad.completed += _ =>
        {
            Plugin.Log.LogInfo($"Scene {sceneName} loaded.");
            var rootGOs = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            switch (sceneName)
            {
                case "2_B-TrashfallArena":
                    ProcessTrashFallArena(rootGOs);
                    break;
                case "2_C-HermitCave":
                    ProcessHermitCave(rootGOs);
                    break;
                default:
                    CacheEnemyAndBossGameObjects(rootGOs);
                    break;
            }

            UnloadScene(sceneName: sceneName);
        };
    }

    private static void ProcessTrashFallArena(GameObject[] rootGOs)
    {
        var cutSceneFirthIntro = rootGOs.FirstOrDefault(x => x.name == "Cutscene_Firth_Intro") ??
                                 throw new ArgumentException("Cutscene_Firth_Intro not found.");

        var content = cutSceneFirthIntro.transform.Find("Content")?.gameObject ??
                      throw new ArgumentException("Content not found in Cutscene_Firth_Intro.");

        var trashIslandArena = content.transform.Find("TrashIsland_Arena_3")?.gameObject ??
                               throw new ArgumentException("TrashIslandArena not found in Content");

        var actorsAboveWater = trashIslandArena.transform.Find("-Actors AboveWater")?.gameObject ??
                               throw new ArgumentException("Actors AboveWater not found in Content.");

        var actorsBelowWater = content.transform.Find("-Actors BelowWater")?.gameObject ??
                               throw new ArgumentException("Actors BelowWater not found in Content.");

        var fakeFirth = actorsAboveWater.transform.Find("Boss_Firth_Fake")?.gameObject ??
                        throw new ArgumentException("Fake Firth not found in Actors AboveWater.");

        var realFirth = actorsBelowWater.transform.Find("Boss_Firth_Real")?.gameObject ??
                        throw new ArgumentException("Real Firth not found in Actors BelowWater.");

        // TODO: pull this out to own method

        EnemyHelpers.SetAllChildrenProblematicComponents(fakeFirth, false);
        var newFakeFirth = Instantiate(fakeFirth);
        DontDestroyOnLoad(newFakeFirth);
        EnemyHelpers.SetAllChildrenProblematicComponents(newFakeFirth, false);
        s_enemySpawner!._cachedBosses.TryAdd(newFakeFirth.name, newFakeFirth);


        EnemyHelpers.SetAllChildrenProblematicComponents(realFirth, false);
        var newRealFirth = Instantiate(realFirth);
        DontDestroyOnLoad(newRealFirth);
        EnemyHelpers.SetAllChildrenProblematicComponents(newRealFirth, false);
        s_enemySpawner!._cachedBosses.TryAdd(newRealFirth.name, newRealFirth);
    }

    private static void ProcessHermitCave(GameObject[] rootGOs)
    {
        try
        {
            var pickups = rootGOs.FirstOrDefault(x => x.name == "Pickups") ??
                          throw new ArgumentException("Pickups not found.");
            var bossesContainer = pickups.transform.Find("Bosses")?.gameObject ??
                                  throw new ArgumentException("Bosses not found in Pickups.");
            var bossGameObjects = TraverseGameObjectHierarchyToFindAll(bossesContainer, typeof(Boss));

            foreach (var boss in bossGameObjects)
            {
                EnemyHelpers.SetAllChildrenProblematicComponents(boss, false);
                var newInstance = Instantiate(boss);
                DontDestroyOnLoad(newInstance);
                EnemyHelpers.SetAllChildrenProblematicComponents(newInstance, false);
                s_enemySpawner!._cachedBosses.TryAdd(newInstance.name, newInstance);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning($"FAILED TO GET PETROCH: {e.Message}");
        }
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

    private static void RecursiveTraverseGameObjectHierarchyToFindAll(
        GameObject container,
        List<Type> componentTypesToCheck,
        ref Dictionary<Type, List<GameObject>>? targetDictionary)
    {
        targetDictionary ??= componentTypesToCheck.ToDictionary(type => type, _ => new List<GameObject>());

        var matchedType = componentTypesToCheck.FirstOrDefault(type => container.GetComponent(type) != null);
        if (matchedType != null)
        {
            targetDictionary[matchedType].Add(container);
            return;
        }

        foreach (Transform child in container.transform)
        {
            var childGameObject = child.gameObject;
            matchedType = componentTypesToCheck.FirstOrDefault(type => childGameObject.GetComponent(type) != null);

            if (matchedType != null)
            {
                targetDictionary![matchedType].Add(childGameObject);
            }
            else
            {
                foreach (Transform nestedChild in childGameObject.transform)
                {
                    RecursiveTraverseGameObjectHierarchyToFindAll(nestedChild.gameObject, componentTypesToCheck, ref targetDictionary);
                }
            }
        }
    }

    private static bool BasicCacheEnemyAndBossGameObjects(GameObject[] rootGOs)
    {
        Dictionary<Type, List<GameObject>> targetGameObjects = new()
        {
            { typeof(Boss), [] },
            { typeof(SaveStateKillableEntity), [] }
        };

        return true;
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

        TraverseContainersForMatchingObjects(enemiesContainer, typeof(SaveStateKillableEntity),
            s_enemySpawner!._cachedEnemies);
        // We also need to traverse the boss containers for regular enemies. For example the ceviche sisters are regular enemies with a different boss container....
        TraverseContainersForMatchingObjects(bossesContainer, typeof(Boss), s_enemySpawner!._cachedBosses);
        TraverseContainersForMatchingObjects(bossesContainer, typeof(SaveStateKillableEntity),
            s_enemySpawner!._cachedEnemies);

        // Remove any gameobjects from _cachedEnemies that are also in _cachedBosses
        foreach (var boss in s_enemySpawner!._cachedBosses)
        {
            s_enemySpawner?._cachedEnemies.TryRemove(boss.Key, out _);
        }

        return true;
    }

    private static List<GameObject> TraverseGameObjectHierarchyToFindAll(GameObject container,
        Type componentTypeToCheck)
    {
        var targetChildren = new List<GameObject>();

        if (container.GetComponent(componentTypeToCheck))
        {
            Plugin.Log.LogWarning($"Identified {container.name} as a {componentTypeToCheck.Name}.");
            targetChildren.Add(container);
            return targetChildren;
        }

        foreach (object? child in container.transform)
        {
            var childGameObject = ((Transform)child).gameObject;

            if (childGameObject.GetComponent(componentTypeToCheck) == null)
            {
                foreach (object? nestedChild in childGameObject.transform)
                {
                    Plugin.Log.LogWarning(
                        $"Scanning nested children of {childGameObject.name} for {componentTypeToCheck.Name}.");
                    var nestedMatchingGameObjects =
                        TraverseGameObjectHierarchyToFindAll(((Transform)nestedChild).gameObject, componentTypeToCheck);
                    Plugin.Log.LogWarning(
                        $"Found {nestedMatchingGameObjects.Count} nested children of {childGameObject.name} for {componentTypeToCheck.Name}.");
                    targetChildren.AddRange(nestedMatchingGameObjects);
                }
            }
            else
            {
                Plugin.Log.LogWarning($"Identified {childGameObject.name} as a {componentTypeToCheck.Name}.");
                targetChildren.Add(childGameObject);
            }
        }

        return targetChildren;
    }

    private static void TraverseContainersForMatchingObjects(List<GameObject> containers, Type componentTypeToCheck,
        ConcurrentDictionary<string, GameObject> targetDictionary)
    {
        List<GameObject> targetChildren = [];
        foreach (var container in containers)
        {
            Plugin.Log.LogWarning($"Traversing {container.name} for {componentTypeToCheck.Name}.");
            targetChildren.AddRange(TraverseGameObjectHierarchyToFindAll(container, componentTypeToCheck));
        }

        Plugin.Log.LogWarning($"Found {targetChildren.Count} {componentTypeToCheck.Name} in scene!!!!!");

        foreach (var childGameObject in targetChildren)
        {
            // We do this since we don't want the instantiated GameObject to spawn its IK holders and other things.
            EnemyHelpers.SetAllChildrenProblematicComponents(childGameObject, false);
            var newInstance = Instantiate(childGameObject);
            DontDestroyOnLoad(newInstance);
            EnemyHelpers.SetAllChildrenProblematicComponents(newInstance,
                false); // As a precaution but could maybe be removed
            targetDictionary.TryAdd(newInstance.name, newInstance);
        }
    }
}
