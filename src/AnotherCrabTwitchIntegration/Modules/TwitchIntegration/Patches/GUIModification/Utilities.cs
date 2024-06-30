/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.TwitchIntegration.Patches.GUIModification;

using UnityEngine.Events;

public static class Utilities
{
    public static void DisableAllListeners(UnityEvent unityEvent)
    {
        int eventCount = unityEvent.GetPersistentEventCount();
        for (int i = 0; i < eventCount; i++)
        {
            unityEvent.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
        unityEvent.RemoveAllListeners();
    }
}
