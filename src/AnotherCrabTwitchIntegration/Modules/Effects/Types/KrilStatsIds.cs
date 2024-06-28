/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Types;

public static class KrilStatsIds
{
    public const string Vitality = "Level_VIT";
    public const string Defense = "Level_DEF";
    public const string Attack = "Level_ATK";
    public const string Magic = "Level_MSG";
    public const string Resistance = "Level_RES";

    public static string GetStatShortHand(string stat)
    {
        return stat switch
        {
            Vitality => "vit",
            Defense => "def",
            Attack => "att",
            Magic => "msg",
            Resistance => "res",
            _ => null
        };
    }

    public static string GetStatName(string stat)
    {
        return stat switch
        {
            "Level_VIT" => "Vitality",
            "Level_DEF" => "Defense",
            "Level_ATK" => "Attack",
            "Level_MSG" => "Magic",
            "Level_RES" => "Resistance",
            _ => null
        };
    }
}