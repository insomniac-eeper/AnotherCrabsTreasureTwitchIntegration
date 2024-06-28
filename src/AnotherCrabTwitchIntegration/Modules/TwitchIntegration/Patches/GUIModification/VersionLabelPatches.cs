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
    public static readonly object Lock = new object();
    public static string VersionPrepend = "TwitchIntegration";
    public static string originalVersion = string.Empty;
    public static TextMeshProUGUI VersionNumber;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VersionText), "Start")]
    public static void VersionLabel_Start_Postfix(VersionText __instance)
    {
        if (!__instance.gameObject.transform.parent.gameObject.name.Contains("MainMenu")) return;

        lock (Lock)
        {
            if (string.IsNullOrEmpty(originalVersion))
            {
                originalVersion = __instance.versionNumber.text;
            }

            __instance.versionNumber.text = $"{VersionPrepend}\n{originalVersion}";
            VersionNumber = __instance.versionNumber;
        }
    }

    public static void SetVersionPrepend(string text)
    {
        lock (Lock)
        {
            VersionPrepend = text;
            try
            {
                if (VersionNumber != null)
                {
                    VersionNumber.text = $"{VersionPrepend}\n{originalVersion}";
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to set Version {ex}");
            }
        }
    }
}