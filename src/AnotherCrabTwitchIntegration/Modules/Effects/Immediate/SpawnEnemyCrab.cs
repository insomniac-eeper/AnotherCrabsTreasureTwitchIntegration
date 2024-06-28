/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEnemyCrab() : EffectDefinition(
    "spawnenemycrab",
    "Spawn Enemy Crab",
    "Spawn an enemy crab.",
    0,
    DoEffect)
{
    private static bool DoEffect()
    {
        var playerLoc = Player.singlePlayer.krilTransform.position;

        var sceneCount = SceneManager.sceneCount;
        for (var i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            Plugin.Log.LogError($"Scene: {scene.name}");

            var rootGameObjects = scene.GetRootGameObjects();
            Plugin.Log.LogError($"  Root Game Objects: {rootGameObjects.Length}");
            foreach (var singleRootGameObject in rootGameObjects)
            {
                if (!(singleRootGameObject.name.Contains("Enemies") || singleRootGameObject.name.Contains("Bosses")))
                {
                    continue;
                }

                foreach (var childTransform in singleRootGameObject.transform)
                {
                    var childGameObject = ((Transform)childTransform).gameObject;
                    Plugin.Log.LogError($"   - {childGameObject.name}");
                }
                Plugin.Log.LogError($"   - {singleRootGameObject.name}");
            }

        }

        //Resources.Load("Enemy_Rangoon_Normie");

        //Player.singlePlayer.StartHackySackAfk();
        return true;
    }
}