/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Effects;

using Modules.Effects.Types;

public class SpawnBoss : EffectDefinition
{
    private readonly Func<bool>? _spawnFunction;
    private readonly string _bossName;

    protected SpawnBoss(string bossName, Func<bool>? spawnFunction = null) : base(
        $"spawn{bossName.ToLower().Replace(" ", string.Empty)}",
        $"Spawn {bossName}",
        $"Spawns the boss {bossName} at the player's location.",
        15,
        null)
    {
        _spawnFunction = spawnFunction;
        OnStartEffect += DoEffect;
    }

    private bool DoEffect()
    {
        try
        {
            return _spawnFunction?.Invoke() ?? false;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to spawn {_bossName}: {ex.Message}");
            return false;
        }
    }
}
