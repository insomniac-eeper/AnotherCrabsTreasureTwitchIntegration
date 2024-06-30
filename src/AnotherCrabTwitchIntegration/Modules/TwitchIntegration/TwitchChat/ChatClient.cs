/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.TwitchChat;

using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using Types;

public class ChatClient(string twitchUsername, string channelName, string oAuthToken)
    : IDisposable
{
    private TwitchClient? _client;
    private WebSocketClient? _webSocketClient;

    private ChatState State { get; set; } = ChatState.Created;

    public Action<ChatMessageRecord>? OnMessage;
    public Action<ChatState>? OnStateUpdate;

    public void Initialize()
    {
        var credentials = new ConnectionCredentials(twitchUsername, oAuthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        _webSocketClient = new WebSocketClient(clientOptions);

        _client = new TwitchClient(_webSocketClient);
        _client.Initialize(credentials, channelName);

        _client.OnLog += OnLog;
        _client.OnJoinedChannel += OnJoinedChannel;
        _client.OnMessageReceived += OnMessageReceived;
        _client.OnWhisperReceived += OnWhisperReceived;
        _client.OnConnected += OnConnected;
        _client.OnDisconnected += OnDisconnected;

        State = ChatState.Initialized;
        OnStateUpdate?.Invoke(State);
    }

    private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
    {
        State = ChatState.Disconnected;
        OnStateUpdate?.Invoke(State);
    }

    public void Connect()
    {
        _client?.Connect();
    }

    public void Disconnect()
    {
        _client?.Disconnect();
    }

    private void OnConnected(object sender, OnConnectedArgs e)
    {
        State = ChatState.Connected;
        OnStateUpdate?.Invoke(State);
        Plugin.Log.LogInfo("Connected to Twitch");
    }

    private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        Plugin.Log.LogDebug($"Received whisper: {e.WhisperMessage.Message} from {e.WhisperMessage.Username}");
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        Plugin.Log.LogInfo($"Received message: {e.ChatMessage.Message} from {e.ChatMessage.Username}");
        OnMessage?.Invoke(new ChatMessageRecord(
            Username: e.ChatMessage.Username,
            Message: e.ChatMessage.Message));
    }

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Plugin.Log.LogInfo($"Joined channel {e.Channel}");
    }

    private void OnLog(object sender, OnLogArgs e)
    {
        Plugin.Log.LogDebug($"Twitch log: {e.Data}");
    }

    public void Dispose()
    {
        _webSocketClient?.Dispose();
    }
}
