/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record struct OauthRequest(
    string client_id,
    List<string> scopes,
    string device_code,
    string grant_type = @"urn:ietf:params:oauth:grant-type:device_code")
{
    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>  {
            { nameof(client_id), client_id },
            { nameof(scopes), string.Join(" ", scopes) },
            { nameof(device_code), device_code },
            { nameof(grant_type), grant_type }
        };
    }
}