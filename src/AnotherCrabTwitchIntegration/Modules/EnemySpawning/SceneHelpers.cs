/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneHelpers
{
    private record struct SceneInfo(string Name, string Path);
    private static List<SceneInfo> GetAllScenes()
    {
        List<SceneInfo> scenes = [];
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string? scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string? sceneName = Path.GetFileNameWithoutExtension(scenePath);
            scenes.Add(new SceneInfo(sceneName, scenePath));
        }

        return scenes;
    }
}
