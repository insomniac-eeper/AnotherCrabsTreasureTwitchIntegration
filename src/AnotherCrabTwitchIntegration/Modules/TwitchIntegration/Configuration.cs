/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration;

using BepInEx.Configuration;

public class Configuration
{
    // [General]
    public ConfigEntry<bool> IsEnabled;
    public ConfigEntry<bool> ShowVersionAndStateOnTitle;

    public void BindToConfig(ConfigFile configFile)
    {
        if (configFile == null)
        {
            return;
        }

        IsEnabled = configFile.Bind("TwitchIntegration", "IsEnabled", true,
            "Whether the Twitch integration module is enabled.");
        ShowVersionAndStateOnTitle = configFile.Bind("TwitchIntegration", "ShowVersionAndStateOnTitle", true,
            "Whether to show the version and state of the Twitch integration on the title screen.");
    }
}