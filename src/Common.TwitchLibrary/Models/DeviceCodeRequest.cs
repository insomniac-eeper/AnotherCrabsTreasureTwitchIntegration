/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record struct DeviceCodeRequest(string client_id, List<string> scopes)
{
    public Dictionary<string, string> ToDictionary()
    {
        return new()
        {
            { nameof(client_id), client_id },
            { nameof(scopes), string.Join(" ", scopes) },
        };
    }
}