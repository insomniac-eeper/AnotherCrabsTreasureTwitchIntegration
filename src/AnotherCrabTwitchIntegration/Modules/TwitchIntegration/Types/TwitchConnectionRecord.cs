/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Types;

using Common.TwitchLibrary.Models;
using TwitchChat;

public record struct TwitchConnectionRecord(
    string Username,
    string Channel,
    AuthState AuthenticationState,
    OauthInformationRecord? OAuthInformation,
    ChatState ChatState);