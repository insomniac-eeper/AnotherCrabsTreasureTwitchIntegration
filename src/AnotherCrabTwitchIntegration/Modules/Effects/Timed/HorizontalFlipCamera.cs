/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using System;
using Postprocessing;
using Types;

public class HorizontalFlipCamera: TimedEffectDefinition
{

    public HorizontalFlipCamera() : base(
        id: "horizontalflip",
        name: "Flip the camera horizontally",
        description: "Flip the camera horizontally",
        cooldownInSeconds: 10,
        duration: 10,
        startEffect: null,
        endEffect: null
        )
    {
        OnStartEffect += DoStartEffect;
        OnEndEffect += DoEndEffect;
    }

    private Guid _guid;

    private bool DoStartEffect()
    {
        _guid = CustomPostProcessing.AddCameraEffect(CameraEffectEnum.HorizontalFlip);
        return true;
    }

    private bool DoEndEffect()
    {
        CustomPostProcessing.RemoveCameraEffect(_guid);
        return true;
    }
}