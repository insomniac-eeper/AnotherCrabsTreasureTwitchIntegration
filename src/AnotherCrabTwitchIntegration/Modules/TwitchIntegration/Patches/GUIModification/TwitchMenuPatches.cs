/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Patches.GUIModification;

using System;
using Common.TwitchLibrary.Models;
using Extensions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch]
public class TwitchMenuPatches
{

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(StartScreen), nameof(StartScreen.Start))]
    // ReSharper disable once InconsistentNaming
    public static bool StartScreen_Start_Prefix(StartScreen __instance)
    {
        var optionsWindow = __instance.optionsWindow.gameObject;

        var newTab = CreateNewTab(optionsWindow);
        var newTabSubSettings = newTab.GetComponent<SettingsSubWindow>();
        var optionsMenu = optionsWindow.GetComponent<OptionsMenu>();
        int tabIdx = optionsMenu.tabs.IndexOf(newTabSubSettings);

        CreateNewTabButton(optionsWindow, tabIdx);

        Plugin.Log.LogDebug("StartScreen.Awake() called.");

        return true;
    }

    private static void CreateNewTabButton(GameObject optionsWindow, int tabIndex)
    {
        var optionsMenu = optionsWindow.GetComponent<OptionsMenu>();

        var panel = optionsWindow.GetChildWithName("Panel") ??
                    throw new ArgumentException($"{optionsWindow.name} does not have a child with name Panel");
        var mainListLeft = panel.GetChildWithName("Main List (Left)") ??
                    throw new ArgumentException($"{panel.name} does not have a child with name Main List (Left)");
        var mainListLeftOptions = mainListLeft.GetChildWithName("Options") ??
                    throw new ArgumentException($"{mainListLeft.name} does not have a child with name Options");

        var buttonList = mainListLeftOptions.GetComponent<ButtonList>();

        var firstMainListLeftOption = mainListLeftOptions.transform.GetChild(0).gameObject ??
                                      throw new ArgumentException($"{mainListLeftOptions.name} does not have a child at index 0");
        var newLeftOption = UnityEngine.Object.Instantiate(firstMainListLeftOption, mainListLeftOptions.transform);
        newLeftOption.name = "Twitch Integration";

        optionsMenu.tabButtons.Add(newLeftOption);

        var newLeftOptionTitleText = newLeftOption.GetChildWithName("TitleText") ??
                                     throw new ArgumentException($"{newLeftOption.name} does not have a child with name TitleText");

        // newLeftOptionTitleText has a LocalizedText component that we need to change
        var tabButtonTitleText = newLeftOptionTitleText.GetComponent<LocalizedText>();
        tabButtonTitleText.index = "Twitch_Integration";
        tabButtonTitleText.RefreshText();

        var tabButtonTitleTMP = newLeftOptionTitleText.GetComponent<TextMeshProUGUI>();

        tabButtonTitleTMP.text = tabButtonTitleText.GetText();

        var tabButtonFlag = newLeftOption.GetComponent<ButtonFlag>();
        tabButtonFlag.onSelect.RemoveAllListeners();
        tabButtonFlag.onClick.RemoveAllListeners();
        tabButtonFlag.onEnter.RemoveAllListeners();

        var newLeftOptionButton = newLeftOption.GetComponent<Button>();

        buttonList.listElements.Add(newLeftOptionButton);
        buttonList.activeList.Add(newLeftOptionButton);

        // Disable all previous listeners which would open the old tab

        Utilities.DisableAllListeners(tabButtonFlag.onSelect);

        tabButtonFlag.onSelect.AddListener(() =>
        {
            // This needs to be OpenTab and not SelectTab
            optionsMenu.OpenTab(tabIndex); // This index needs to be the index of the new tab
        });

        // Note that both the new tab button and new tab need to be added to the OptionsMenu instance
    }

    private static GameObject CreateNewTab(GameObject optionsWindow)
    {
        var twitchIntegration = GetTwitchIntegration();

        var optionsMenu = optionsWindow.GetComponent<OptionsMenu>();
        var panel = optionsWindow.GetChildWithName("Panel") ??
                    throw new ArgumentException($"{optionsWindow.name} does not have a child with name Panel");
        var subListRight = panel.GetChildWithName("Sub List (Right)") ??
                    throw new ArgumentException($"{panel.name} does not have a child with name Sub List (Right)");
        var gameOptions = subListRight.GetChildWithName("GameplayOptions") ??
                    throw new ArgumentException($"{subListRight.name} does not have a child with name GameplayOptions");
        var newTab = UnityEngine.Object.Instantiate(gameOptions, subListRight.transform);

        newTab.name = "TwitchIntegrationOptions";

        var newTabSettingsSubWindow = newTab.GetComponent<SettingsSubWindow>();
        // Needs to be set otherwise we get spammed with FMOD errors. Looks to not be copied properly
        newTabSettingsSubWindow.openSound = "UI/UI_Scroll_List";
        optionsMenu.tabs.Add(newTabSettingsSubWindow);

        var firstOption = newTab.GetChildWithName("Language_Enum") ??
                          throw new ArgumentException($"{newTab.name} does not have a child with name Language_Enum");
        firstOption.name = "Connect_Twitch";

        StripOptionButton(firstOption);
        PrepareConnectButton(firstOption);

        var secondOption = newTab.transform.GetChild(1).gameObject;
        var thirdOption = newTab.transform.GetChild(2).gameObject;

        for (int i = 3; i < newTab.transform.childCount; i++)
        {
            UnityEngine.Object.Destroy(newTab.transform.GetChild(i).gameObject);
        }

        secondOption.name = "TwitchConnectionState";
        StripOptionToggle(secondOption);

        var secondOptionTitleText = secondOption.GetChildWithName("TitleText") ??
                                    throw new ArgumentException($"{secondOption.name} does not have a child with name TitleText");
        var secondOptionText = secondOptionTitleText.GetComponent<TextMeshProUGUI>();

        twitchIntegration.UpdateState += state => { secondOptionText.text = state; };
        twitchIntegration.UpdateState("Not connected");
        thirdOption.name = "TwitchConnectionUser";

        StripOptionToggle(thirdOption);

        var thirdOptionTitleText = thirdOption.GetChildWithName("TitleText") ??
                                   throw new ArgumentException($"{thirdOption.name} does not have a child with name TitleText");
        var thirdOptionText = thirdOptionTitleText.GetComponent<TextMeshProUGUI>();

        thirdOptionText.text = "User: N/A";

        twitchIntegration.UpdateUser += user => { thirdOptionText.text = $"Authorized Username: {user}"; };


        twitchIntegration.InitializeConnectButtonManager(firstOption);

        twitchIntegration.UpdateState("Checking for previous auth...");
        twitchIntegration.ConnectButtonBehaviorManager?
            .SetGameButtonState(GameButtonState.WaitingBlocked, "CHECKING...");

        Plugin.Log.LogInfo($"Current Twitch state: {twitchIntegration.TwitchConnectionRecord}");

        twitchIntegration.UpdateUser("N/A");
        switch (twitchIntegration.TwitchConnectionRecord.AuthenticationState)
        {
            case AuthState.Authenticated:
                twitchIntegration.ConnectButtonBehaviorManager?.SetGameButtonState(GameButtonState.ConnectedBlocked);
                twitchIntegration.UpdateUser(twitchIntegration.TwitchConnectionRecord.Username);
                break;
            case AuthState.NotAuthenticated:
                twitchIntegration.ConnectButtonBehaviorManager?.SetGameButtonState(GameButtonState.ReadyToStartDeviceRequest);
                break;
            case AuthState.AwaitingUserAuthorization:
                twitchIntegration.ConnectButtonBehaviorManager?.SetGameButtonState(GameButtonState.ReadyToStartOAuthRequest);
                break;
            case AuthState.Error: // Yes this is a conflation of error and out of range... I ought to rework this
            default:
                throw new ArgumentOutOfRangeException(twitchIntegration.TwitchConnectionRecord.AuthenticationState.ToString());
        }


        return newTab;
    }

    private static void PrepareConnectButton(GameObject btn)
    {
        var firstOptionTitleText = btn.GetChildWithName("TitleText") ??
                                   throw new ArgumentException($"{btn.name} does not have a child with name TitleText");
        var firstOptionText = firstOptionTitleText.GetComponent<TextMeshProUGUI>();
        firstOptionText.text = "CONNECT TO TWITCH";

        var firstOptionButton = btn.GetChildWithName("Button") ??
                                throw new ArgumentException($"{btn.name} does not have a child with name Button");

        var firstOptionButtonText = firstOptionButton.GetChildWithName("Text") ??
                                    throw new ArgumentException($"{firstOptionButton.name} does not have a child with name Text");
        var firstOptionButtonTextTMP = firstOptionButtonText.GetComponent<TextMeshProUGUI>();
        firstOptionButtonTextTMP.text = "CONNECT";
    }

    private static void StripOptionButton(GameObject btn)
    {
        UnityEngine.Object.Destroy(btn.GetComponent<ButtonFlag>());
        var firstOptionTitleText = btn.GetChildWithName("TitleText") ??
                                   throw new ArgumentException($"{btn.name} does not have a child with name TitleText");

        UnityEngine.Object.Destroy(firstOptionTitleText.GetComponent<LocalizedText>());

        // firstOptionButton has Button component with logic
        var firstOptionButton = btn.GetChildWithName("Button") ??
                                throw new ArgumentException($"{btn.name} does not have a child with name Button");
        UnityEngine.Object.Destroy(btn.GetComponent<OptionsMenuOption>());
        var firstOptionButtonText = firstOptionButton.GetChildWithName("Text") ??
                                    throw new ArgumentException($"{firstOptionButton.name} does not have a child with name Text");
        UnityEngine.Object.Destroy(firstOptionButtonText.GetComponent<LocalizedText>());

        var firstButtonObj = btn.GetComponent<Button>();

        Utilities.DisableAllListeners(firstButtonObj.onClick);
    }

    private static void StripOptionToggle(GameObject toggle)
    {
        UnityEngine.Object.Destroy(toggle.GetComponent<Button>());
        UnityEngine.Object.Destroy(toggle.GetComponent<Animator>());
        UnityEngine.Object.Destroy(toggle.GetComponent<ButtonFlag>());
        UnityEngine.Object.Destroy(toggle.GetComponent<OptionsMenuOption>());
        UnityEngine.Object.Destroy(toggle.GetChildWithName("Background"));
        UnityEngine.Object.Destroy(toggle.GetChildWithName("Fill"));

        var thirdOptionTitleText = toggle.GetChildWithName("TitleText") ??
                                   throw new ArgumentException($"{toggle.name} does not have a child with name TitleText");
        UnityEngine.Object.Destroy(thirdOptionTitleText.GetComponent<LocalizedText>());
    }

    private static TwitchIntegration? s_twitchIntegration;

    private static TwitchIntegration GetTwitchIntegration()
    {
        if (s_twitchIntegration is null)
        {
            var twitchIntegrationGo = GameObject.Find("TwitchIntegration");
            s_twitchIntegration = twitchIntegrationGo.GetComponent<TwitchIntegration>();
        }

        return s_twitchIntegration;
    }

}
