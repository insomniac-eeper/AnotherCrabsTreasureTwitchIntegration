/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.EnemySpawning.Effects;

public class SpawnNephro(EnemySpawner spawner) : SpawnBoss("Nephro", bool () => spawner.SpawnNephro());
public class SpawnVoltai(EnemySpawner spawner) : SpawnBoss("Voltai", bool () => spawner.SpawnVoltai());
public class SpawnFirth(EnemySpawner spawner) : SpawnBoss("Firth", bool () => spawner.SpawnFirthReal());
public class SpawnPetroch(EnemySpawner spawner) : SpawnBoss("Petroch", bool () => spawner.SpawnPetroch());
public class SpawnHeikea(EnemySpawner spawner) : SpawnBoss("Heikea", bool () => spawner.SpawnHeikea());
public class SpawnInkerton(EnemySpawner spawner) : SpawnBoss("Inkerton", bool () => spawner.SpawnInkerton());
public class SpawnConsortium(EnemySpawner spawner) : SpawnBoss("Consortium", bool () => spawner.SpawnConsortium());
public class SpawnLichenthrope(EnemySpawner spawner) : SpawnBoss("Lichenthrope", bool () => spawner.SpawnLichenthrope());
public class SpawnPagurus(EnemySpawner spawner) : SpawnBoss("Pagurus", bool () => spawner.SpawnPagurus());
public class SpawnBruiserBoss(EnemySpawner spawner) : SpawnBoss("BruiserBoss", bool () => spawner.SpawnBruiserBoss());
public class SpawnBruiserGroveBoss(EnemySpawner spawner) : SpawnBoss("BuiserGrove", bool () => spawner.SpawnBruiserGrove());
public class SpawnExecutioner(EnemySpawner spawner) : SpawnBoss("Executioner", bool () => spawner.SpawnExecutioner());
public class SpawnBruiserScuttleport(EnemySpawner spawner) : SpawnBoss("BruiserScuttleport", bool () => spawner.SpawnBruiserScuttleport());
public class SpawnBleachedKing(EnemySpawner spawner) : SpawnBoss("Bleached King", bool () => spawner.SpawnBleachedKing());
public class SpawnMoltedKing(EnemySpawner spawner) : SpawnBoss("Molted King", bool () => spawner.SpawnMoltedKing());
public class SpawnTopoda(EnemySpawner spawner) : SpawnBoss("Topoda", bool () => spawner.SpawnTopoda());
public class SpawnRoland(EnemySpawner spawner) : SpawnBoss("Roland", bool () => spawner.SpawnRoland());

// Mising Magista, Ceviche Sisteres, and Priya Dubia




