/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.TwitchChat;

public enum ChatState
{
    Created,
    Initialized,
    Disconnected,
    Connected,
    Unknown,
}