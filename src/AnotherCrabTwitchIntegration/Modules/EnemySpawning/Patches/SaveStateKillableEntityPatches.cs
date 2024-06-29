/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

using HarmonyLib;

[HarmonyPatch]
public class SaveStateKillableEntityPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveStateKillableEntity), nameof(SaveStateKillableEntity.LoadFromFile))]
    public static bool SaveStateKillableEntity_LoadFromFile_Prefix(SaveStateKillableEntity __instance)
    {
        return __instance.gameObject.GetComponent<CustomSpawn>() == null;
    }
}