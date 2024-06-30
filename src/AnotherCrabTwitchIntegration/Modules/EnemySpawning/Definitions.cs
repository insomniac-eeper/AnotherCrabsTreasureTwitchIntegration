/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using System.Collections.Generic;

public static class Definitions
{
    internal static readonly List<string> s_scenesWithEnemies =
    [
        "2_A-ShallowsTidePools",
        "2_B-ShallowsBigSand",
        "2_D-MoonSnailShellCave",
        "2_A-NCTradeRoute",
        "2_B-NCCity",
        "2_A-OOGroveRadius",
        "2_A-GroveForestLow",
        "2_B-GroveForestHigh",
        "2_C-Village",
        "2_D-Caves",
        "2_E-Cliffs",
        "2_A-HighSwamp",
        "2_B-LowSwamp",
        "2_C-Facilities",
        "2_E-VoltaiArena",
        "2_B-PinBargeArena",
        "2_B-DarkCanyon",
        "2_C-HermitCave",
        "2_D-SilentFlats",
        "2_E-WhaleBoneCave",
        "2_F-WhaleArena",
        "2_A-BleachedCopse",
        "2_B-GrandCourtyard",
        // "2_C-BottomOfTheDrain", // Praya Dubia
        "2_B-TrashfallArena" // FIRTH

    ];

    internal static readonly List<string> s_uniqueEnemies =
    [
        "Enemy_PistolShrimp",
        "Bobbit Worm",
        "Enemy_Swarmite",
        "Enemy_Rangoon_Martini",
        "Enemy_Sardine_2 Variant",
        "Enemy_BoxCrabBaby Variant",
        "Enemy_Rangoon_Thimble",
        "Enemy_BleachedRangoon",
        "Enemy_ViperFish_Screamer",
        "Enemy_Rangoon_Normie Lichen Variant",
        "Enemy_Rangoon_molted",
        "Enemy_BleachedSeahorse",
        "Enemy_Sardine",
        "Enemy_Cuttlefish",
        "Enemy_Pufferfish",
        "Anchovy",
        "Enemy_Rangoon_Normie",
        "Enemy_Rangoon_Normie_Stunnable Variant",
        "Enemy_Lobster_Bleached",
        "Enemy_Rangoon_Parasol",
        "Enemy_Frogfish",
        "Rangoon_Normie Grove",
        "Enemy_ViperFish",
        "Enemy_Rangoon_Broadsword",
        "Enemy_BleachedBowman",
        "Enemy_Seahorse",
        "Enemy_Executioner",
        "Enemy_Rangoon_Corkscrew",
        "Enemy_Gumpounder",
        "Enemy_Mimic",
        "Enemy_SilenceBrandCrab",
        "Enemy_FrogAngler",
        "Enemy_Stunpounder",
        "Enemy_Bowman",
        "Enemy_Bruiser",
        "Enemy_Bowman_Elite Variant",
        "Enemy_BobbitWorm_EvilVariant",
        "Enemy_CevicheSister Variant",
        "NPC_RudeSnail",
        "Enemy_Pufferfish_Bleached_NEW Variant",
        "Enemy_BleachedRangoon_FatherLevee",
        "Rangoon_Base",
        "Rangoon_Normie"
    ];
}
