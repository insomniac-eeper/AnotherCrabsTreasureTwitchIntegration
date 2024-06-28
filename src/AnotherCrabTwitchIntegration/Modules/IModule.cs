/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules;

public interface IModule
{
    public bool IsInitialized { get; }
    public bool IsEnabled { get; }
}