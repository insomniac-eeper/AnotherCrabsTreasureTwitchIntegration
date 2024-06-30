/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Patches.GUIModification;

using System;
using HarmonyLib;
using TMPro;

[HarmonyPatch]
public static class VersionLabelPatches
{
    private static readonly object s_lock = new object();
    private static string s_versionPrepend = "TwitchIntegration";
    private static string s_originalVersion = string.Empty;
    private static TextMeshProUGUI? s_versionNumber;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VersionText), "Start")]
    // ReSharper disable once InconsistentNaming
    public static void VersionLabel_Start_Postfix(VersionText __instance)
    {
        if (!__instance.gameObject.transform.parent.gameObject.name.Contains("MainMenu")) return;

        lock (s_lock)
        {
            if (string.IsNullOrEmpty(s_originalVersion))
            {
                s_originalVersion = __instance.versionNumber.text;
            }

            __instance.versionNumber.text = $"{s_versionPrepend}\n{s_originalVersion}";
            s_versionNumber = __instance.versionNumber;
        }
    }

    public static void SetVersionPrepend(string text)
    {
        lock (s_lock)
        {
            s_versionPrepend = text;
            try
            {
                if (s_versionNumber != null)
                {
                    s_versionNumber.text = $"{s_versionPrepend}\n{s_originalVersion}";
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to set Version {ex}");
            }
        }
    }
}
