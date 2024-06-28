/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Models;

public enum AuthState
{
    NotAuthenticated,
    AwaitingUserAuthorization,
    Authenticated,
    Error,
}