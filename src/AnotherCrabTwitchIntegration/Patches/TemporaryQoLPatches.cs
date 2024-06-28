/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Patches;

using HarmonyLib;

[HarmonyPatch]
public class TemporaryQoLPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartScreen), nameof(StartScreen.Awake))]
    public static bool UserSettings_InitSettings_Postfix(StartScreen __instance)
    {
        StartScreen.hasPlayedPreTitle = true;
        __instance.waitForPretitleTime = 0f;
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Pretitle), nameof(Pretitle.GoToTitle))]
    public static bool Pretitle_GoToTitle_Prefix()
    {
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.ParseCrabJson))]
    public static void SaveData_ParseCrabJson_Prefix()
    {
        Plugin.Log.LogError("PARSING CRAB JSON");
    }
}