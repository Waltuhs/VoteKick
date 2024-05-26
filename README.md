# VoteKick
![downloads](https://img.shields.io/github/downloads/Waltuhs/VoteKick/total?logo=github&style=for-the-badge)
![ver](https://img.shields.io/github/v/release/Waltuhs/VoteKick?include_prereleases&logo=github&style=for-the-badge)
[![disc](https://img.shields.io/discord/1235681501849321482?label=Discord&logo=discord&style=for-the-badge)](https://discord.gg/MQAcPFJRkR)
## A Scp:Sl Exiled plugin that introduces a counter strike style votekick system into the game.

| Command | What it does |
| --- | --- |
| .vk start %playername goes here% | starts a new votekick against the specified playername |
| .vk yes | votes yes on the active votekick |
| .vk no | votes no on the active votekick |

keep in mind all of these commands must be entered into the client side console (button above tab/ ` )

# Config
### global cooldown is available in the plugins config (default is 20 seconds)
### votekick duration is also configuarble in config (default is 30 seconds)
### the percent of the lobby needing to vote yes in order to kick the player (default 25%)
### in the config you can also blacklist certain "roles" also known as groups from being votekicked along with blacklisting certain steamids from starting a votekick and being votekicked

default config
``` yml
VK:
# is the plugin enabled?
  is_enabled: true
  # is the Debug mode enabled?
  debug: false
  # Global VoteKick Cooldown (int) (default 20)
  cooldown: 20
  # Votekick total time (int) (default 30)
  v_k_time: 30
  # How much percent of the current players ingame need to vote yes for a kick (0.25 = 25%) (default 25%/0.25)
  list_percent: 0.25
  # List of SteamIDs that cannot start a votekick
  blacklisted_initiators: []
  # List of SteamIDs that cannot be votekicked
  blacklisted_targets: []
  # List of roles that cannot be votekicked
  blacklisted_roles: []
  ```
  Example config
  ``` yml
VK:
# is the plugin enabled?
  is_enabled: true
  # is the Debug mode enabled?
  debug: false
  # Global VoteKick Cooldown (int) (default 20)
  cooldown: 20
  # Votekick total time (int) (default 30)
  v_k_time: 30
  # How much percent of the current players ingame need to vote yes for a kick (0.25 = 25%) (default 25%/0.25)
  list_percent: 0.25
  # List of SteamIDs that cannot start a votekick
  blacklisted_initiators: 
  - '12345@steam'
  # List of SteamIDs that cannot be votekicked
  blacklisted_targets:
  - '12345@steam'
  # List of roles that cannot be votekicked
  blacklisted_roles:
  - 'owner'
  ```




