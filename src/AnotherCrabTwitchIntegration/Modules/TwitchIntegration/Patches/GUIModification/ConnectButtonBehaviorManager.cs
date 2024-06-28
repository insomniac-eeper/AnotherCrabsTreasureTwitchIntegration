/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Patches.GUIModification;

using System;
using System.Threading.Tasks;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectButtonBehaviorManager
{
    private Button _connectButtonComponent;
    private TextMeshProUGUI _buttonText;
    private Button.ButtonClickedEvent OnClickEvent;

    private TwitchIntegration _twitchIntegration;

    private Action<string> _updateStateText;

    public GameButtonState CurrentState { get; private set; }

    public ConnectButtonBehaviorManager(GameObject btn, TwitchIntegration twitchIntegration, Action<string> updateStateText = null)
    {
        _twitchIntegration = twitchIntegration;
        _updateStateText = updateStateText;

        var firstOptionButton = btn.GetChildWithName("Button");
        var firstOptionsButtonObj = btn.GetComponent<Button>();
        _connectButtonComponent = firstOptionsButtonObj;
        OnClickEvent = firstOptionsButtonObj.onClick;

        var firstOptionButtonText = firstOptionButton.GetChildWithName("Text");
        var firstOptionButtonTextTMP = firstOptionButtonText.GetComponent<TextMeshProUGUI>();
        _buttonText = firstOptionButtonTextTMP;
    }

    private void UpdateText(string newText)
    {
        try
        {
            _buttonText.text = newText;
        } catch (Exception ex)
        {
            Plugin.Log.LogError($"Error updating text: {ex}");
        }
    }

    private void UpdateStateText(string newText)
    {
        try
        {
            _updateStateText?.Invoke(newText);
        } catch (Exception ex)
        {
            Plugin.Log.LogError($"Error updating state text: {ex}");
        }
    }

    private void UpdateOnClickBehavior(Action newBehavior)
    {
        try
        {
            // Woo memory leaks :P
            Utilities.DisableAllListeners(OnClickEvent);
            OnClickEvent.AddListener(() => Task.Run(newBehavior));
        } catch (Exception ex)
        {
            Plugin.Log.LogError($"Error updating on click behavior: {ex}");
        }
    }

    public void SetGameButtonState(GameButtonState state, string optionalTextOverride = "")
    {
        Plugin.Log.LogError($"Current state: {CurrentState}, new state: {state}");
        try
        {
            if (_connectButtonComponent == null)
            {
                Plugin.Log.LogError("ConnectButtonComponent is null....");
            }
            switch (state)
            {
                case GameButtonState.WaitingBlocked:
                    UpdateText("WAITING...");
                    UpdateStateText("Waiting for response...");
                    UpdateOnClickBehavior(null);
                    _connectButtonComponent.enabled = false;
                    CurrentState = state;
                    break;
                case GameButtonState.ReadyToStartDeviceRequest:
                    UpdateText("CONNECT");
                    UpdateStateText("Ready to request device code.");
                    UpdateOnClickBehavior(RequestDeviceCodeBehavior);
                    _connectButtonComponent.enabled = true;
                    CurrentState = state;
                    break;
                case GameButtonState.ReadyToStartOAuthRequest:
                    UpdateText("REQUEST OAUTH");
                    UpdateStateText("Ready to request OAuth code.");
                    UpdateOnClickBehavior(RequestOAuthBehavior);
                    _connectButtonComponent.enabled = true;
                    CurrentState = state;
                    break;
                case GameButtonState.ConnectedBlocked:
                    UpdateText("DISCONNECT");
                    UpdateStateText("Successfully connected.");
                    UpdateOnClickBehavior(DisconnectBehavior);
                    Plugin.Log.LogError("Setting connected blocked state.");
                    Plugin.Log.LogError($"connectButtonComponent: {_connectButtonComponent}");
                    _connectButtonComponent.enabled = true;
                    CurrentState = state;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Error setting game button state: {ex}");
        }
    }

    private void DisconnectBehavior()
    {
        _twitchIntegration.Disconnect();
        SetGameButtonState(GameButtonState.ReadyToStartDeviceRequest);
    }

    private void RequestDeviceCodeBehavior()
    {
        UpdateText("REQUESTING...");

        _connectButtonComponent.enabled = false;

        UpdateStateText("Requesting device code...");
        var deviceCodeResponse = _twitchIntegration.RequestDeviceCode();
        if (deviceCodeResponse == null)
        {
            Plugin.Log.LogError("Error requesting device code");
            SetGameButtonState(GameButtonState.ReadyToStartDeviceRequest);
            return;
        }

        UpdateText("Got a device code, opening verification URL...");
        Application.OpenURL(deviceCodeResponse.verification_uri);
        SetGameButtonState(GameButtonState.ReadyToStartOAuthRequest);
    }

    private void RequestOAuthBehavior()
    {
        UpdateText("REQUESTING...");
        _connectButtonComponent.enabled = false;

        if (_twitchIntegration.RequestOAuth())
        {
            _twitchIntegration.OnAuthSuccess();
            SetGameButtonState(GameButtonState.ConnectedBlocked);
            return;
        }

        Plugin.Log.LogError("Error requesting OAuth");
        SetGameButtonState(GameButtonState.ReadyToStartDeviceRequest);
    }
}