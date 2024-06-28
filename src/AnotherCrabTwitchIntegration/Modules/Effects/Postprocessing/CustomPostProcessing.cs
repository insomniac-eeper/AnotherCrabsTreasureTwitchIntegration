/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Postprocessing;

using System;
using System.Collections.Concurrent;
using UnityEngine;

public class CustomPostProcessing : MonoBehaviour
{
    private static bool _doInvert;

    private static readonly ConcurrentDictionary<Guid, CameraEffectEnum> EffectDict = new();
    public Camera mCamera;

    void OnPreCull()
    {

        mCamera.ResetWorldToCameraMatrix();
        mCamera.ResetProjectionMatrix();

        if (EffectDict.Count == 0)
        {
            return;
        }

        foreach (var effect in EffectDict.Values)
        {
            TransformCamera(effect);
        }
    }

    void OnPreRender()
    {
        // Need to also make sure that pause menu krill doesn't get inverted....
        GL.invertCulling = _doInvert;
    }

    void OnPostRender()
    {
        // GL.invertCulling = !_doInvert;
    }

    public static Guid AddCameraEffect(CameraEffectEnum effectEnum)
    {
        _doInvert = !_doInvert;
        var id = Guid.NewGuid();
        EffectDict[id] = effectEnum;
        return id;
    }

    public static void RemoveCameraEffect(Guid id)
    {
        while(EffectDict.ContainsKey(id))
        {
            EffectDict.TryRemove(id, out _);
        }
        _doInvert = !_doInvert;
    }

    private void TransformCamera(CameraEffectEnum effect)
    {
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