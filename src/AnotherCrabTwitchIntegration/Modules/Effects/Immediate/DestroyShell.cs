/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using System;
using Types;

public class DestroyShell() : EffectDefinition(
    "destroyshell",
    "Destroy Shell",
    "Destroy the player's shell if one is equipped.",
    15,
    DoEffect)
{

    private static bool DoEffect()
    {
        try
        {
            if (Player.singlePlayer.equippedShell is not null)
            {
                Player.singlePlayer.equippedShell.TakeDamage(100000f);
            }
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to destroy shell: {ex.Message}");
            return false;
        }
    }
}