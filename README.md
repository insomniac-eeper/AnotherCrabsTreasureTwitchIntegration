# Another Crab's Treasure Twitch Integration

## Description

This mod adds Twitch integrations for the game [Another Crab's Treasure](https://store.steampowered.com/app/1572210/Another_Crabs_Treasure/).
It is still very much a WIP and I would not yet consider it ready for public use.

## Dependencies
- MonoMod RunTime Detours
  - Ensure you have the [BepInEx.MonoMod.HookGenPatcher](https://github.com/harbingerofme/Bepinex.Monomod.HookGenPatcher) in your patcher directory. 
- Create a secrets.env file next to secrets.env.example and fill in the values with your registered application Twitch Client ID.

## Important Notes
- This mod is not yet ready for public use.
- If you are using this mod **BACKUP YOUR SAVE FILE**
- You must register your own Twitch application in order to use the Twitch connectivity.
  - Ensure that you create a secrets.env file next to secrets.env.example and fill in the values with your registered application Twitch Client ID.
- Twitch Oauth Renew flow is broken for confidential client IDs. This means that after the 4 hours timeout you may need to reauth.... This is a known issue in the Twitch API
- This version currently **does not filter requests by username**. This means that anyone writing in chat can raise effects.
- If you want to test effects without having to authenticate with Twitch you can connect to the Websocket server by connecting to {BASEURL}/ws (default is http://127.0.0.1:12345/ws).
  -  Simply send the effect activation ID as a message.
- Overlay is currently WIP so do not expect it to work properly.
- In order to be able to spawn enemies I have to go through all the scenes with enemies and cache them.
  - This means if the EnemySpawningModule is active and config option AutoTrawlAtStart is set to true the game will load into the title and then lag heavily until the caching is done and Title scene is reloaded.
    - If you want to avoid this set AutoTrawlAtStart to false. However **Enemy spawning will not work if you do this**. 
- You can refer to the configuration file generated once the mod is loaded (BepInEx/config/{NameOfMod}.cfg) and mess around with the settings there.
- **ANY MODS THAT AFFECT INITIALIZATION ORDER LIKE SKIP THE INTRO MAY BREAK THINGS**.
  - For example [ACTQoL](https://thunderstore.io/c/another-crabs-treasure/p/Voidlings/ACTQoL/) which skips the Pre-Title.
  - I have to make the initialization a bit more resilient so it can handle these cases. 
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
- Random shell attached to weapon (Not added yet)
- Add Status Ailment
  - Fear
  - Gunk
  - Hypnosis
  - Scour
  - Bleach
- Random Shell
- Break Shell
- Break Weapon Shell (Not added yet)
- Start Afk routine
- Modify Stats (add/dec by one)
- Modify Microplastics (Not added yet)
- Add Stowaways (Not added yet)
- Modify SkillPoints (Not added yet)
- Modify Stainless Steel Relics (Not added yet)
- Give consumeables (Not added yet)
  - Items that can be sold for microplastics
  - Items that can upgrade your stats
- Spawn Enemies (Not added yet)
- Spawn Bosses
  - Topoda
  - Heikea

#### Timed Effects
- Speed Boost
- Speed Slowdown
- Invincibility
- Invisible
- Gun Time
- Do no damage
- Flip camera horizontally
- Flip camera vertically
- Invert camera
- No blocking
- No jumping
- No running
- One hit kill

## TODOS
- [x] Define Effects and Timed Effects
- [x] Add Menu item and dialog to start and configure integration
- [x] Add webserver to show an overlay of active and queued effects
- [x] Add storage of refresh token / access token
- [ ] Add effect triggers
  - [x] Based on chat messages
  - [ ] Based on poll results
  - [ ] Based on channel points redemptions
  - [ ] Based on bit donations
- [ ] Add text above enemy heads
  - [ ] Get chatter/sub list to display above enemy heads
- [x] Allow Boss Spawns
- [ ] Add remaining Boss spawns
- [ ] Make overlay pretty and animated
- [ ] Save file protection (Copy and load/write save from new alternate location automatically)
- [ ] Add authorized user definition
- [ ] Make configuration nicer and depend on BepInEx config concept
- [ ] Rework effect overrides
- [ ] Make enemy trawling invokable
- [ ] Add warning/message/etc during trawling process
- [ ] Cull cached enemy GameObjects to be unique
- [ ] Ensure non-standardly-defined bosses and enemies are also cached
  - Firth
  - Electric Eel
  - etc

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
- [Fody](https://github.com/Fody/Fody)
  - MIT License Copyright (c) Simon Cropp
- [Fody.Costura](https://github.com/Fody/Costura)
  - MIT License Copyright (c) 2012 Simon Cropp and contributors
- [Anime.js](https://github.com/juliangarnier/anime/)
  - MIT License Copyright (c) 2023 Julian Garnier
- [LethalCompanyTemplate](https://github.com/Distractic/LethalCompanyTemplate)
  - MIT License Copyright (c) 2023 Lethal Company Community