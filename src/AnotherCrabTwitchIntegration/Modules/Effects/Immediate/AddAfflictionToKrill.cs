/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class AddAfflictionToKrill : EffectDefinition
{
    private readonly HitEvent.AFFLICTIONTYPE _afflictionType;

    protected AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE afflictionType, int cooldown) : base(
        $"{afflictionType.ToString().ToLower()}",
        $"Apply {afflictionType.ToString().ToLower()} to Krill",
        $"Apply {afflictionType.ToString().ToLower()} to Krill",
        cooldown)
    {
        _afflictionType = afflictionType;
        OnStartEffect += DoEffect;
    }

    private bool DoEffect()
    {
        if (_afflictionType == HitEvent.AFFLICTIONTYPE.NONE)
        {
            Plugin.Log.LogError($"Attempted to apply an invalid affliction to Krill.");
            return false;
        }

        Player.singlePlayer.TakeAffliction(_afflictionType, 99999f);
        return true;
    }
}
