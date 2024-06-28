/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.Effects.Immediate;

public class Gunk() : AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE.GUNK, 15);
public class Fear() : AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE.FEAR, 15);
public class Scour() : AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE.SCOUR, 15);
public class Bleach() : AddAfflictionToKrill(HitEvent.AFFLICTIONTYPE.BLEACH, 15);
