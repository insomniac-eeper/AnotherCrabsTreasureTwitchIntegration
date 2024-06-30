/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

using HarmonyLib;

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

[HarmonyPatch]
public static class BossPatches
{
    /// <summary>
    /// Prevents bosses with a CustomSpawn component from saving progress
    /// </summary>
    /// <param name="__instance">The instance of the component we are targeting.</param>
    /// <returns><c>true</c> if original prefix should run, <c>false</c> otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Boss), nameof(Boss.SaveProgress))]
    // ReSharper disable once InconsistentNaming
    public static bool Boss_SaveProgress_Prefix(Boss __instance)
    {
        return __instance.gameObject.GetComponent<CustomSpawn>() == null;
    }
}
