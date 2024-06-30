/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class ModifyKrillStat : EffectDefinition
{
    private readonly string _id;
    private readonly int _amount;

    protected ModifyKrillStat(string id, int amount, int cooldown): base(
    $"{(amount > 0 ? "add" : "dec")}{KrilStatsIds.GetStatShortHand(id)}",
    $"{(amount > 0 ? "Add" : "Remove")} {amount} {KrilStatsIds.GetStatName(id)}",
    $"{(amount > 0 ? "Add" : "Remove")} {amount} {KrilStatsIds.GetStatName(id)}",
    cooldown) {
        this._id = id;
        this._amount = amount;
        OnStartEffect += DoEffect;
    }


    private bool DoEffect()
    {
        if (string.IsNullOrEmpty(_id))
        {
            Plugin.Log.LogError($"Failed to modify stat: {_id} is not a valid stat.");
            return false;
        }
        CrabFile.current.inventoryData.AdjustAmount(_id, _amount);
        Player.singlePlayer.playerStatBlock.Init();
        return true;
    }
}
