/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Patches;

using System;using HarmonyLib;

[HarmonyPatch]
public class StartScreenPatches
{
    internal static Action? s_onTitleLoad;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartScreen), nameof(StartScreen.Init))]
    public static void StartScreen_Init_Postfix()
    {
        s_onTitleLoad?.Invoke();
    }
}
