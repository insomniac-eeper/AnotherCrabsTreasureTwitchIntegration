/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Interfaces;

public interface ITwitchChatHandler
{
    public void InitializeChatClient(string twitchUsername, string oAuthToken, string channelName);
    public void Disconnect();
}