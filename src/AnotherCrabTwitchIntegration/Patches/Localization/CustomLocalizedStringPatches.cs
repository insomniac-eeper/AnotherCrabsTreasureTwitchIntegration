/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Patches;

using System.Linq;
using HarmonyLib;

[HarmonyPatch]
public class CustomLocalizedStringPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalizedText), nameof(LocalizedText.LoadMainTable))]
    public static void LocalizedText_LoadMainTable_Postfix()
    {
        LocalizedText.MAIN_TABLE.Add("TWITCH_INTEGRATION", Enumerable.Repeat("Twitch Integration", 12).ToList());
        LocalizedText.MAIN_TABLE.Add("TWITCH_INTEGRATION_CONNECT", Enumerable.Repeat("Connect to Twitch", 12).ToList());
    }
}