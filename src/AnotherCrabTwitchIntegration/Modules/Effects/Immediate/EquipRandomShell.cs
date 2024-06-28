/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using System;
using System.Collections.Generic;
using System.Linq;
using Types;
using Random = UnityEngine.Random;

public class EquipRandomShell() : EffectDefinition(
    "randomshell",
    "Equip Random Shell",
    "Equip a random shell.",
    15,
    DoEffect)
{
    private static List<ShellCollectable> _shellCollectables;
    private static List<ShellCollectable> ShellCollectables => _shellCollectables ??= InventoryMasterList.staticList.OfType<ShellCollectable>().ToList();

    private static bool DoEffect()
    {
        try
        {
            var randomShell = ShellCollectables[Random.Range(0, ShellCollectables.Count)];
            Plugin.Log.LogError($"Random shell: {randomShell.name}");
            var equipShell = randomShell.Equip();
            Plugin.Log.LogError($"Equipped: {equipShell}");
            Player.singlePlayer.SwapShell(equipShell, true, true);
            Plugin.Log.LogError("Shell Swapped");
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}