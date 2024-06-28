/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record struct OauthRefreshRequestResponse(
    string access_token,
    string refresh_token,
    List<string> scope,
    string token_type);