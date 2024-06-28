/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record OauthResponse(
    string access_token,
    int expires_in,
    string refresh_token,
    List<string> scope,
    string token_type) : ExpirableResponse(expires_in);