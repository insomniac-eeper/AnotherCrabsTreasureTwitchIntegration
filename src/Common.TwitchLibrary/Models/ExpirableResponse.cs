/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record ExpirableResponse
{
    internal readonly long CreatedAtTimestamp;
    internal readonly int ExpiresIn;

    protected ExpirableResponse(int expiresIn)
    {
        ExpiresIn = expiresIn;
        CreatedAtTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public bool IsExpired => DateTimeOffset.Now.ToUnixTimeSeconds() - CreatedAtTimestamp >= ExpiresIn;

}