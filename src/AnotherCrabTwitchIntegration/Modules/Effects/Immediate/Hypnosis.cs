// /*
//  * SPDX-License-Identifier: GPL-3.0
//  * Another Crab's Treasure Twitch Integration
//  * Copyright (c) 2024 insomniac-eeper and contributors
//  */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using System.Linq;
using Types;
using UnityEngine;

public class Hypnosis() : EffectDefinition(
    "hypnosis",
    "Afflict Krill with hypnosis",
    "Krill will be hypnotized by the nearest entity.",
    15,
    DoEffect)
{
    private static bool DoEffect()
    {
        var player = Player.singlePlayer;

        var closestEnemy = Enemy.allEnemies
            .Where(x => x.dead == false)
            .Where(x => Vector3.Angle(player.VectorToEntity(x), player.view.transform.forward) < 95f)
            .OrderBy( x => Vector3.Distance(x.transform.position, player.transform.position))
            .FirstOrDefault();

        if (closestEnemy == null)
        {
            Plugin.Log.LogError($"Unable to find closest enemy to player.");
            return false;
        }

        player.TakeAffliction(HitEvent.AFFLICTIONTYPE.HYPNOSIS, 99999f, closestEnemy);
        return true;
    }
}