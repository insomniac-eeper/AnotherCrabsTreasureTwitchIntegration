/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record struct OauthRefreshRequestRecord(
    string client_id,
    string refresh_token,
    string grant_type = "refresh_token")
{
    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "grant_type", grant_type },
            { "refresh_token", refresh_token },
            { "cliend_id", client_id },
        };
    }
}