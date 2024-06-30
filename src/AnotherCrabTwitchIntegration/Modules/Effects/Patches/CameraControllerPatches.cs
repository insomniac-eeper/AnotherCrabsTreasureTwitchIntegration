/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Patches;

using System;
using HarmonyLib;
using Postprocessing;

[HarmonyPatch]
public class CameraControllerPatches
{

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))]
    // ReSharper disable once InconsistentNaming
    public static void CameraController_Start_Postfix(CameraController __instance)
    {
        var mainCamera = __instance.cam;
        var mainCamGo = mainCamera.gameObject;

        if (mainCamGo.GetComponent<CustomPostProcessing>() == null)
        {
            Plugin.Log.LogDebug("CameraController_Start_Postfix: Adding CustomPostProcessing");
            try
            {
                var customPostProcessing = mainCamGo.AddComponent<CustomPostProcessing>();
                customPostProcessing.mCamera = __instance.cam;
            } catch (Exception e)
            {
                Plugin.Log.LogError("CameraController_Start_Postfix: Error adding CustomPostProcessing: " + e);
            }

        }
    }

}
