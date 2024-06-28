/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

using Types;

public class AddVit() : ModifyKrillStat(KrilStatsIds.Vitality, 1, 15);
public class RemVit() : ModifyKrillStat(KrilStatsIds.Vitality, -1, 15);
public class AddDef() : ModifyKrillStat(KrilStatsIds.Defense, 1, 15);
public class RemDef() : ModifyKrillStat(KrilStatsIds.Defense, -1, 15);
public class AddAtk() : ModifyKrillStat(KrilStatsIds.Attack, 1, 15);
public class RemAtk() : ModifyKrillStat(KrilStatsIds.Attack, -1, 15);
public class AddMsg() : ModifyKrillStat(KrilStatsIds.Magic, 1, 15);
public class RemMsg() : ModifyKrillStat(KrilStatsIds.Magic, -1, 15);
public class AddRes() : ModifyKrillStat(KrilStatsIds.Resistance, 1, 15);
public class RemRes() : ModifyKrillStat(KrilStatsIds.Resistance, -1, 15);