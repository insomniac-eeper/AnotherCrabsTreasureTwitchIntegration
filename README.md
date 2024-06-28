# Another Crab's Treasure Twitch Integration

## Description

## Dependencies
- MonoMod RunTime Detours

## Features

### Effects
There are two types of effects: timed and immediate.
Timed effects are effects that affect the game over a predetermined amount of time.
Immediate effects are effects that change the game state as soon as they are active and take no other action.
An example of a timed effect is a speed boost that can last (as an example) 15 seconds.
An example of an immediate effect is taking damage.

#### Immediate Effects
- Take Damage
- Set HP to 1
- Die
- Use Heartkelp Sprout
- Refill Heartkelp Sprouts
- Empty Heartkelp Sprouts
- Random shell attached to weapon
- Add Status Ailment
  - Fear
  - Gunk
  - Venom
  - Hypnosis
  - Scour
  - Bleach
- Random Shell
- Break Shell
- Break Weapon Shell

#### Timed Effects
- Speed Boost
- Speed Slowdown
- Invincibility
- Invisible
- Gun Time
- God mode

## TODOS
- [ ] Define Effects and Timed Effects
- [ ] Add Menu item and dialog to start and configure integration
- [ ] Add webserver to show an overlay of active and queued effects
- [ ] Add storage of refresh token / access token
- [ ] Add effect triggers
  - [ ] Based on chat messages
  - [ ] Based on poll results
  - [ ] Based on channel points redemptions
  - [ ] Based on bit donations
- [ ] Add text above enemy heads
  - [ ] Get chatter/sub list to display above enemy heads
- [ ] Allow Boss Spawns

## Acknowledgements and Attributions
- [EmbedIO](https://github.com/unosquare/embedio)
 - MIT License Copyright (c) 2014-2019 Unosquare, Mario A. Di Vece and Geovanni Perez
- [TwitchLib](https://github.com/TwitchLib/TwitchLib/)
 - MIT License Copyright (c) 2017 swiftyspiffy (Cole)
- [DotEnvGenerator](https://github.com/bolorundurowb/dotenv.net)
 - MIT License Copyright (c) 2017 Bolorunduro Winner-Timothy B
- [BepInEx](https://github.com/BepInEx/BepInEx)
 - LGPL-2.1 License Copyright (c) 2019 BepInEx and contributors
- [MonoMod](https://github.com/MonoMod/MonoMod)
 - MIT License Copyright (c) 2015 - 2020 0x0ade 