/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Types;

public record struct SerializedAuthDataRecord(
    string OAuthToken,
    long OAuthIssueDateEpochSeconds,
    long OAuthTokenExpirationInSeconds,
    string OAuthRefreshToken,
    string TwitchUsername,
    string TwitchChannel)
{
};