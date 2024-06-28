/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Extensions;

using System.Collections.Concurrent;

public static class ConcurrentStackExtensions
{
    public static void SpinTryPop<T>(this ConcurrentStack<T> stack, out T result)
    {
        bool popped;
        do
        {
            popped = stack.TryPop(out result);
        } while (!popped && stack.Count > 0);
    }
}