/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.ChannelPoints;

using System;

public class ChannelPointRewardsManager
{
    private readonly TwitchIntegration _twitchIntegration;

    public ChannelPointRewardsManager(TwitchIntegration twitchIntegration)
    {
        _twitchIntegration = twitchIntegration;
    }

    public bool CreateChannelPointRewards()
    {
        throw new NotImplementedException();
        return true;
    }

}