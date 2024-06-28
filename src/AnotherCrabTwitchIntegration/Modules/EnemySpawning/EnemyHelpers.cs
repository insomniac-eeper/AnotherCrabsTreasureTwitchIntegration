/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning;

using UnityEngine;

public class EnemyHelpers
{
    internal static void SetAllChildrenProblematicComponents(GameObject o, bool enabled)
    {
        o.SetActive(enabled);
        foreach (var childTransform in o.transform)
        {
            SetAllChildrenProblematicComponents(((Transform)childTransform).gameObject, enabled);
        }

        var components = o.GetComponents<Component>();
        foreach (var component in components)
        {
            switch (component)
            {
                case ProceduralLegController legController:
                    legController.enabled = enabled;
                    break;
                case DynamicBone bone:
                    bone.enabled = enabled;
                    break;
                case Enemy enemy:
                    enemy.enabled = enabled;
                    break;
                case LerpTransformToPlayer lerp:
                    lerp.enabled = enabled;
                    break;
            }
        }
    }
}