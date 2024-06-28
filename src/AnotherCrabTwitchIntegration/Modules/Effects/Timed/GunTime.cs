/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Timed;

using Types;

public class GunTime() :
    TimedEffectDefinition(
        "guntime",
        "Gun Time",
        "Temporarily give Kril a gun",
        cooldownInSeconds: 20,
        duration: 10,
        startEffect: DoStartEffect,
        endEffect: DoEndEffect)
{

    private static bool DoStartEffect()
    {
        Player.singlePlayer.EquipGun();
        return true;
    }

    private static bool DoEndEffect()
    {
        var player = Player.singlePlayer;
        (CrabFile.current.inventoryData["Inv_Shell_Gun"].LookupItem() as ShellCollectable)?.UninsureShell();
        if (!player.HasEquippedShell || player.equippedShell.stats.shellCollectable.name != "Inv_Shell_Gun")
        {
            return true;
        }

        player.RemoveAndDestroyShell();
        var collectableItemData = CrabFile.current.inventoryData.startingShell.LookupItem();
        if (collectableItemData != null && collectableItemData.name == "Inv_Shell_Gun")
        {
            CrabFile.current.inventoryData.startingShell = null;
        }
        return true;
    }
}