/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Services;

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

using Models;
using Utilities;

public class AuthService : IAuthService
{
    private readonly HttpClientWrapper _httpClientWrapper;
    private readonly string _twitchClientId;
    private readonly List<string> _scopes;

    private DeviceCodeResponse? _deviceCodeResponse;
    private OauthResponse? _oauthResponse;

    public OauthInformationRecord? OAuthInformation { get; private set; }

    public AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public AuthService(
        string twitchClientId,
        List<string>? scopes = null,
        HttpClientWrapper? httpClientWrapper = null)
    {
        if (string.IsNullOrEmpty(twitchClientId))
        {
            throw new ArgumentNullException(nameof(twitchClientId));
        }

        _twitchClientId = twitchClientId;

        if (scopes is null || scopes.Count == 0)
        {
            _scopes = Constants.DefaultScopes;
        }
        else
        {
            _scopes = scopes;
        }

        _httpClientWrapper = httpClientWrapper ?? new HttpClientWrapper();
    }

    public bool RenewOAuth()
    {
        var request = new OauthRefreshRequestRecord(_twitchClientId, OAuthInformation?.RefreshToken ?? string.Empty);
        var formParams = new FormUrlEncodedContent(request.ToDictionary());
        var response = _httpClientWrapper.PostAsync(Constants.DcfOauthUrl, formParams).GetAwaiter().GetResult();

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Status code: {response.StatusCode}, Reason: {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
        }

        var oauthRenewResponse =
            JsonSerializer.Deserialize<OauthRefreshRequestResponse>(response.Content.ReadAsStringAsync().GetAwaiter()
                .GetResult());

        if (oauthRenewResponse == null)
        {
            return false;
        }

        this.OAuthInformation = new OauthInformationRecord(
            AccessToken: oauthRenewResponse.access_token,
            RefreshToken: oauthRenewResponse.refresh_token,
            IssueDateEpochSeconds: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExpirationFromIssueDateInSeconds: (long)TimeSpan.FromHours(4).TotalSeconds);
        return true;
    }

    public void SetState(AuthState state, OauthInformationRecord? oauthInformation = null)
    {
        AuthState = state;
        OAuthInformation = oauthInformation;
    }

    public async Task<DeviceCodeResponse?> RequestDeviceCode()
    {
        var request = new DeviceCodeRequest(_twitchClientId, _scopes);
        var formParams = new FormUrlEncodedContent(request.ToDictionary());
        var response = await _httpClientWrapper.PostAsync(Constants.DcfAuthUrl, formParams);

        response.EnsureSuccessStatusCode();

        var deviceCodeResponse = JsonSerializer.Deserialize<DeviceCodeResponse>(await response.Content.ReadAsStringAsync());
        _deviceCodeResponse = deviceCodeResponse;

        if (deviceCodeResponse is not null)
        {
            AuthState = AuthState.AwaitingUserAuthorization;
        }

        return deviceCodeResponse;
    }

    public async Task<OauthResponse?> RequestAuthCode(string deviceCode = "")
    {
        if (string.IsNullOrEmpty(deviceCode))
        {
            deviceCode = _deviceCodeResponse?.device_code ?? string.Empty;
        }

        var request = new OauthRequest(_twitchClientId, _scopes, deviceCode);
        var formParams = new FormUrlEncodedContent(request.ToDictionary());
        var response = await _httpClientWrapper.PostAsync(Constants.DcfOauthUrl, formParams);

        response.EnsureSuccessStatusCode();

        var authResponse = JsonSerializer.Deserialize<OauthResponse>(await response.Content.ReadAsStringAsync());
        _oauthResponse = authResponse;

        if (authResponse is not null)
        {
            AuthState = AuthState.Authenticated;
            OAuthInformation = new(
                _oauthResponse!.access_token,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                _oauthResponse.expires_in,
                _oauthResponse.refresh_token);
        }

        return authResponse;
    }

    public bool CheckAuthentication()
    {
        if (OAuthInformation == null || string.IsNullOrEmpty(OAuthInformation?.AccessToken))
        {
            return false;
        }

        var request = new HttpRequestMessage
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("OAuth", OAuthInformation?.AccessToken)
            },
            Method = HttpMethod.Get,
            RequestUri = new(Constants.OAuthValidateUrl)
        };

        var validateTask = _httpClientWrapper.SendAsync(request);

        var validateResult = validateTask.GetAwaiter().GetResult();
        return validateResult.StatusCode == HttpStatusCode.OK;
    }
}