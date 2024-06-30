/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

using AnotherCrabTwitchIntegration.Modules.Effects.Patches;

namespace AnotherCrabTwitchIntegration.Modules.Effects.Postprocessing;

using System;
using System.Collections.Concurrent;
using UnityEngine;

public class CustomPostProcessing : MonoBehaviour
{
    private static bool s_doInvert;

    private static readonly ConcurrentDictionary<Guid, CameraEffectEnum> s_effectDict = new();
    public Camera? mCamera;

    void OnPreCull()
    {

        mCamera?.ResetWorldToCameraMatrix();
        mCamera?.ResetProjectionMatrix();

        if (s_effectDict.Count == 0)
        {
            return;
        }

        foreach (var effect in s_effectDict.Values)
        {
            TransformCamera(effect);
        }
    }

    void OnPreRender()
    {
        // Need to also make sure that pause menu krill doesn't get inverted....
        GL.invertCulling = s_doInvert;
    }

    public static Guid AddCameraEffect(CameraEffectEnum effectEnum)
    {
        s_doInvert = !s_doInvert;
        var id = Guid.NewGuid();
        s_effectDict[id] = effectEnum;
        return id;
    }

    public static void RemoveCameraEffect(Guid id)
    {
        while(s_effectDict.ContainsKey(id))
        {
            s_effectDict.TryRemove(id, out _);
        }
        s_doInvert = !s_doInvert;
    }

    private void TransformCamera(CameraEffectEnum effect)
    {
        if (mCamera == null)
        {
            Plugin.Log.LogError($"{nameof(mCamera)} has not yet been set by {nameof(CameraControllerPatches)}...");
            return;
        }

        mCamera.projectionMatrix *= effect switch
        {
            CameraEffectEnum.HorizontalFlip => Matrix4x4.Scale(new Vector3(-1, 1, 1)),
            CameraEffectEnum.VerticalFlip => Matrix4x4.Scale(new Vector3(1, -1, 1)),
            CameraEffectEnum.BothFlip => Matrix4x4.Scale(new Vector3(-1, -1, 1)),
            CameraEffectEnum.Invert => Matrix4x4.Scale(new Vector3(-1, -1, -1)),
            _ => Matrix4x4.Scale(new Vector3(1, 1, 1))
        };
    }

}
