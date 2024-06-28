/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using BepInEx.Configuration;

public class Configuration
{
    // [General]
    public ConfigEntry<bool> IsEnabled;
    public ConfigEntry<bool> AutoTrawlAtStart;

    public void BindToConfig(ConfigFile configFile)
    {
        if (configFile == null)
        {
            return;
        }

        IsEnabled = configFile.Bind("EnemySpawning", "IsEnabled", true, "Whether the enemy spawning module is enabled.");
        AutoTrawlAtStart = configFile.Bind("EnemySpawning", "AutoTrawlAtStart", true, "Whether to automatically start the trawl at the beginning of the game.");
    }
}