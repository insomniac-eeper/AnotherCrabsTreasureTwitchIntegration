/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public record DeviceCodeResponse(
    string device_code,
    int expires_in,
    int interval,
    string user_code,
    string verification_uri,
    long startingTime) : ExpirableResponse(expires_in);