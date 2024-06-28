/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Extensions;

using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject GetChildWithName(this GameObject parent, string name)
    {
        return (parent.transform.Cast<Transform>()
            .Where(child => child.gameObject.name == name)
            .Select(child => child.gameObject)).FirstOrDefault();
    }

    public static void RemoveAllChildren(this GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}