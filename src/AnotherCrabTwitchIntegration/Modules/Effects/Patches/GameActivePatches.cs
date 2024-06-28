/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches;

using System.Threading;
using HarmonyLib;

[HarmonyPatch]
public class GameActivePatches
{
    public static int IsGameActive;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.StartNewPlaySession))]
    public static void GameManager_StartNewPlaySession()
    {
        Interlocked.Exchange(ref IsGameActive, 1);
        Plugin.Log.LogError("GameManager_StartNewPlaySession");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PauseGame))]
    public static bool GameManager_PauseGame(ref bool active)
    {
        Interlocked.Exchange(ref IsGameActive, active ? 0 : 1);
        return true;
    }
}