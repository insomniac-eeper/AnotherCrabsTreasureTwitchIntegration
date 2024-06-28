/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Utilities;

public static class Constants
{
    internal const string DcfAuthUrl = "https://id.twitch.tv/oauth2/device";
    internal const string DcfOauthUrl = "https://id.twitch.tv/oauth2/token";
    internal const string OAuthValidateUrl = "https://id.twitch.tv/oauth2/validate";
    internal static readonly List<string> DefaultScopes = ["chat:read", "chat:edit"];
}