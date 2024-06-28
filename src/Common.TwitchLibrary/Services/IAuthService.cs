/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Services;

using Models;

public interface IAuthService
{
    AuthState AuthState { get; }
    public OauthInformationRecord? OAuthInformation { get; }
    Task<DeviceCodeResponse?> RequestDeviceCode();
    Task<OauthResponse?> RequestAuthCode(string deviceCode = "");

    bool CheckAuthentication();

    bool RenewOAuth();

    void SetState(AuthState state, OauthInformationRecord? oauthInformation = null);
}