/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.TwitchLibrary.Models;
using Common.TwitchLibrary.Services;
using DotEnv.Generated;
using Effects;
using Interfaces;
using Patches.GUIModification;
using TwitchChat;
using TwitchLib.Api;
using Types;
using UnityEngine;

public class TwitchIntegration : MonoBehaviour, ITwitchChatHandler, ITwitchAuthenticator
{
    public EffectIngress EffectIngress { get; private set; }
    private ChatClient? _chatClient;

    private string AuthSaveFile { get; } = Path.Combine(Environment.CurrentDirectory, "twitchAuthData.json");

    private TwitchConnectionRecord _twitchConnectionRecord = new(
        null, null, AuthState.NotAuthenticated, null, ChatState.Unknown);

    public TwitchConnectionRecord TwitchConnectionRecord
    {
        get => _twitchConnectionRecord;
        private set
        {
            _twitchConnectionRecord = value;
            OnConnectionStateChange?.Invoke(this, _twitchConnectionRecord);
        }
    }

    public Action<string> UpdateState;
    public Action<string> UpdateUser;
    public ConnectButtonBehaviorManager ConnectButtonBehaviorManager { get; private set; }

    public IAuthService AuthService { get; private set; }
    public TwitchAPI Api;

    public event EventHandler<TwitchConnectionRecord> OnConnectionStateChange;

    public void Initialize(EffectIngress? effectIngress = null)
    {
        EffectIngress = effectIngress ?? throw new ArgumentException($"{nameof(effectIngress)} is null.");
        var twitchClientId = SecretsEnvironment.TwitchClientId ?? string.Empty;
        AuthService = new AuthService(twitchClientId);
        Api = new TwitchAPI();
    }

    private void SetAuthenticatedState()
    {
        TwitchConnectionRecord = TwitchConnectionRecord with
        {
            AuthenticationState = AuthState.Authenticated
        };

        AuthService.SetState(AuthState.Authenticated, TwitchConnectionRecord.OAuthInformation);
    }

    private void SetUnauthenticatedState()
    {
        Plugin.Log.LogError("Unable to re-authenticate");
        TwitchConnectionRecord = new TwitchConnectionRecord(
            null, null, AuthState.NotAuthenticated, null, ChatState.Unknown);
    }

    private void InitializeAuthState()
    {
        Task.Run(() =>
        {
            if (!CheckForPreviousAuth()) return;

            if (ValidateAuthentication() || TryReAuthenticate())
            {
                SetAuthenticatedState();
                InitializeChatClient(
                    TwitchConnectionRecord.Username,
                    TwitchConnectionRecord.OAuthInformation?.AccessToken,
                    TwitchConnectionRecord.Channel);
            }
            else
            {
                SetUnauthenticatedState();
            }
        });
    }

    private void Start()
    {
        try
        {
            InitializeAuthState();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Unable to start TwitchIntegration: {e}");
        }

        Plugin.Log.LogInfo("TwitchIntegration started");
    }

    public void InitializeConnectButtonManager(GameObject btn)
    {
        ConnectButtonBehaviorManager = new ConnectButtonBehaviorManager(btn, this, UpdateState);
    }

    private SerializedAuthDataRecord? ParseSaveDateForPreviousAuth()
    {
        if (!File.Exists(AuthSaveFile))
        {
            Plugin.Log.LogWarning("No previous auth data found");
            return null;
        }

        var authDataRaw = File.ReadAllText(AuthSaveFile);
        return JsonSerializer.Deserialize<SerializedAuthDataRecord>(authDataRaw);
    }

    private void AddAuthDataToSavedData()
    {
        var authData = new SerializedAuthDataRecord(
            TwitchUsername: TwitchConnectionRecord.Username,
            TwitchChannel: TwitchConnectionRecord.Channel,
            OAuthToken: TwitchConnectionRecord.OAuthInformation?.AccessToken,
            OAuthIssueDateEpochSeconds: TwitchConnectionRecord.OAuthInformation?.IssueDateEpochSeconds ?? 0,
            OAuthTokenExpirationInSeconds: TwitchConnectionRecord.OAuthInformation?.ExpirationFromIssueDateInSeconds ??
                                           0,
            OAuthRefreshToken: TwitchConnectionRecord.OAuthInformation?.RefreshToken);

        Plugin.Log.LogInfo("Adding auth data to json file");
        // Serialize the authData to json and then save it to a json file in the same location
        var serializedData = JsonSerializer.Serialize(authData);
        File.WriteAllText(AuthSaveFile, serializedData);
    }

    public bool CheckForPreviousAuth()
    {
        var previousAuthData = ParseSaveDateForPreviousAuth();

        if (previousAuthData == null) return false;

        Plugin.Log.LogInfo("Able to parse date for previous auth");

        Plugin.Log.LogDebug($"Previous auth data: {previousAuthData}");

        var oauthInfo = new OauthInformationRecord(
            previousAuthData?.OAuthToken,
            previousAuthData?.OAuthIssueDateEpochSeconds ?? 0,
            previousAuthData?.OAuthTokenExpirationInSeconds ?? 0,
            previousAuthData?.OAuthRefreshToken);

        TwitchConnectionRecord = new TwitchConnectionRecord(
            previousAuthData?.TwitchUsername,
            previousAuthData?.TwitchChannel,
            AuthState.NotAuthenticated,
            oauthInfo,
            ChatState.Connected);

        AuthService.SetState(AuthState.NotAuthenticated, oauthInfo);
        return true;
    }

    public bool ValidateAuthentication()
    {
        try
        {
            var isValid = AuthService.CheckAuthentication();
            Plugin.Log.LogInfo($"Authentication validity: {isValid}");
            return isValid;
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error validating authentication: {e}");
        }

        return false;
    }

    public bool TryReAuthenticate()
    {
        Plugin.Log.LogInfo("Attempting to re-authenticate");
        var isCurrentAuthValid = AuthService.CheckAuthentication();
        Plugin.Log.LogInfo($"Current auth validity: {isCurrentAuthValid}");
        if (isCurrentAuthValid) return true;


        var ableToRenew = false;
        try
        {
            ableToRenew = AuthService.RenewOAuth();
        }
        catch (Exception ex)
        {
            Plugin.Log.LogWarning($"Unable to renew oauth: {ex}");
            return false;
        }

        Plugin.Log.LogInfo($"Able to renew authentication: {ableToRenew}");

        if (!ableToRenew) return false;

        TwitchConnectionRecord = TwitchConnectionRecord with
        {
            OAuthInformation = AuthService.OAuthInformation,
            AuthenticationState = AuthState.Authenticated,
        };
        return true;
    }

    private void UpdateChatState(ChatState state)
    {
        TwitchConnectionRecord = TwitchConnectionRecord with { ChatState = state };
    }

    public void InitializeChatClient(string twitchUsername, string oAuthToken, string channelName)
    {
        Plugin.Log.LogInfo("Initializing Twitch Chat client");
        _chatClient?.Disconnect();
        if (_chatClient != null)
        {
            _chatClient.OnStateUpdate -= UpdateChatState;
            _chatClient.OnMessage -= Client_OnMessageReceived;
            _chatClient.Dispose();
        }

        Plugin.Log.LogInfo($"Creating new chat client with username: {twitchUsername}, channel: {channelName}");
        _chatClient = new ChatClient(twitchUsername, channelName, oAuthToken);
        _chatClient.OnStateUpdate += UpdateChatState;
        _chatClient.OnMessage += Client_OnMessageReceived;
        _chatClient.Initialize();
        Plugin.Log.LogInfo("Chat client initialized");
        _chatClient.Connect();
        Plugin.Log.LogInfo("Chat client connected");
    }

    public DeviceCodeResponse RequestDeviceCode()
    {
        DeviceCodeResponse response = null;
        try
        {
            response = AuthService.RequestDeviceCode().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Unable to request device code: {ex}");
            return null;
        }

        if (response == null)
        {
            return null;
        }

        TwitchConnectionRecord = TwitchConnectionRecord with
        {
            AuthenticationState = AuthState.AwaitingUserAuthorization
        };
        return response;
    }

    public bool RequestOAuth()
    {
        OauthResponse response = null;
        try
        {
            response = AuthService.RequestAuthCode().GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning($"Error requesting oauth: {e}");
            return false;
        }

        if (response == null)
        {
            return false;
        }

        TwitchConnectionRecord = TwitchConnectionRecord with
        {
            AuthenticationState = AuthState.Authenticated,
            OAuthInformation = AuthService.OAuthInformation
        };

        return true;
    }

    private string GetUserNameAssociatedWithOAuthToken(string oAuthToken)
    {
        Plugin.Log.LogInfo("Starting Twitch user query");
        var getUsersResponse =
            Api.Helix.Users.GetUsersAsync(accessToken: oAuthToken).GetAwaiter().GetResult();
        Plugin.Log.LogDebug("Queried user for oauth token");
        var user = getUsersResponse.Users.FirstOrDefault();
        Plugin.Log.LogInfo($"Got a user: {user?.DisplayName}");
        UpdateUser?.Invoke(user?.DisplayName ?? "Unknown");
        Plugin.Log.LogDebug("Updated user name");
        return user?.DisplayName;
    }

    public void OnAuthSuccess()
    {
        Plugin.Log.LogDebug("OnAuthSuccess called");
        var oAuthToken = AuthService.OAuthInformation?.AccessToken;

        if (string.IsNullOrEmpty(oAuthToken))
        {
            throw new InvalidOperationException("OAuthToken is null or empty");
        }

        Plugin.Log.LogDebug("OnAuthSuccess setting secrets");
        var twitchClientId = SecretsEnvironment.TwitchClientId ?? string.Empty;
        Api.Settings.ClientId = twitchClientId;
        Api.Settings.Secret = oAuthToken;

        var user = GetUserNameAssociatedWithOAuthToken(oAuthToken);

        if (user != null)
        {
            var oAuthInfo = AuthService.OAuthInformation;
            TwitchConnectionRecord = TwitchConnectionRecord with
            {
                AuthenticationState = AuthState.Authenticated,
                OAuthInformation = oAuthInfo,
                Username = user,
                Channel = user
            };

            AddAuthDataToSavedData();
            InitializeChatClient(user, oAuthToken,
                user);
        }
        else
        {
            Plugin.Log.LogError("User is null on authentication success, this is unexpected....");
        }
    }

    private void ClearAPISecrets()
    {
        Api.Settings.ClientId = string.Empty;
        Api.Settings.Secret = string.Empty;
        Api.Settings.AccessToken = string.Empty;
    }

    public void Disconnect()
    {
        _chatClient?.Disconnect();
        if (_chatClient != null)
        {
            _chatClient.OnStateUpdate -= UpdateChatState;
            _chatClient.OnMessage -= Client_OnMessageReceived;
        }

        ClearAPISecrets();

        TwitchConnectionRecord = new TwitchConnectionRecord(
            OAuthInformation: null,
            Channel: string.Empty,
            AuthenticationState: AuthState.NotAuthenticated,
            Username: string.Empty,
            ChatState: ChatState.Disconnected);

        AuthService.SetState(AuthState.NotAuthenticated);

        UpdateUser?.Invoke("N/A");
        UpdateState?.Invoke("Not Connected...");

        DeleteAuthData();
    }

    private bool DeleteAuthData()
    {
        if (!File.Exists(AuthSaveFile))
        {
            return false;
        }

        try
        {
            File.Delete(AuthSaveFile);
            return true;
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error deleting auth data: {e}");
            return false;
        }
    }

    private void Client_OnMessageReceived(ChatMessageRecord messageRecord)
    {
        EffectIngress.TryAddEffect(messageRecord.Message, messageRecord.Username);
    }
}
