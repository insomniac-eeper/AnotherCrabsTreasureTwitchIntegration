/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Effects;

using System;
using Modules.Effects.Types;

public class SpawnHeikea : EffectDefinition
{
    private readonly EnemySpawner _enemySpawner;

    public SpawnHeikea(EnemySpawner spawner) : base(
        "spawnheikea",
        "Spawn Heikea",
        "Spawns the boss Heikea at the player's location.",
        15,
        null)
    {
        _enemySpawner = spawner;
        OnStartEffect += DoEffect;
    }

    private bool DoEffect()
    {
        if (_enemySpawner == null)
        {
            return false;
        }

        try
        {
            _enemySpawner?.SpawnHeikea();
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to spawn Topoda: {ex.Message}");
            return false;
        }
    }
}