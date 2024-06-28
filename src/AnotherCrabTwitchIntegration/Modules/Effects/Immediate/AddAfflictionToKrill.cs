/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class AddAfflictionToKrill : EffectDefinition
{
    private HitEvent.AFFLICTIONTYPE _afflictiontype;

    protected AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE afflictiontype, int cooldown) : base(
        $"{afflictiontype.ToString().ToLower()}",
        $"Apply {afflictiontype.ToString().ToLower()} to Krill",
        $"Apply {afflictiontype.ToString().ToLower()} to Krill",
        cooldown)
    {
        _afflictiontype = afflictiontype;
        OnStartEffect += DoEffect;
    }

    private bool DoEffect()
    {
        if (_afflictiontype == HitEvent.AFFLICTIONTYPE.NONE)
        {
            Plugin.Log.LogError($"Attempted to apply an invalid affliction to Krill.");
            return false;
        }

        Player.singlePlayer.TakeAffliction(_afflictiontype, 99999f);
        return true;
    }
}