/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class ModifyKrillStat : EffectDefinition
{
    private string id;
    private int amount;

    protected ModifyKrillStat(string id, int amount, int cooldown): base(
    $"{(amount > 0 ? "add" : "dec")}{KrilStatsIds.GetStatShortHand(id)}",
    $"{(amount > 0 ? "Add" : "Remove")} {amount} {KrilStatsIds.GetStatName(id)}",
    $"{(amount > 0 ? "Add" : "Remove")} {amount} {KrilStatsIds.GetStatName(id)}",
    cooldown) {
        this.id = id;
        this.amount = amount;
        OnStartEffect += DoEffect;
    }


    private bool DoEffect()
    {
        if (string.IsNullOrEmpty(id))
        {
            Plugin.Log.LogError($"Failed to modify stat: {id} is not a valid stat.");
            return false;
        }
        CrabFile.current.inventoryData.AdjustAmount(id, amount);
        Player.singlePlayer.playerStatBlock.Init();
        return true;
    }
}