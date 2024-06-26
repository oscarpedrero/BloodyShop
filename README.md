# BloodyShop Mod to create a store in VRising (with optional client UI) Update to VRising 1.0

## IMPORTANT NOTE

- At the moment it only works in the server version, the client mode with the UI is disabled.
- Removed the daily drop system, an external mod has been generated for this called [BloodyRewards](https://thunderstore.io/c/v-rising/p/Trodi/BloodyRewards/)
- You must have version 1.2.4 of Bloody.Core installed to be able to use version 1.0.2 or higher of this mod

# Support this project

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K8ENRQY)

# Requirements

1. [BepInEx](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/) (Client and Server)
2. [Bloodstone](https://thunderstore.io/c/v-rising/p/deca/Bloodstone/) (Client and Server)
3. [VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/) (Only on the Server)
4. [Bloody.Core](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/) (Client and Server)
4. UniverseLib.IL2CPP.Interop.dll include in zip (Only on the client)

<details>
<summary>Changelog</summary>

`1.0.2`
- Performance improvements

`1.0.1`
- Removed the daily drop system, an external mod has been generated for this called BloodyRewards

`1.0.0`
- Removed dependency on V Rising.Game Data
- Added Bloody.Core as a dependency
- Updated to VRising version 1.0

`0.9.92`
- Added to the add and buy command the possibility of adding buffs and not just items. Example: .shop add Cursed 1425734039 1 1 1 1 true

`0.9.9`
- Added command to reload configuration of products and currencies
- Now you can configure if you want a currency to be active or not in the drop system

`0.9.82`
- Fixed bug that did not allow adding infinite items

`0.9.81`
- Fixed a bug that existed when the currency_list.json file was first generated.
- Added option within the client configuration file to disable sounds.

`0.9.8`
- Added UI to set and remove currencies.
- Added messaging system for currencies.
- Optimize the performance of network messages from the server that caused lag to different players.
- Added a user registration system that uses the UI to only notify those users of changes in real time.
- Change of icons of the main menu.
- Refactoring of the window to add or remove products from the store, now everything is displayed in a single window.
- Fixed bug that did not allow deleting products from the UI.
- Added sounds to the UI for certain actions.
- The UniverseLib.IL2CPP.Interop.dll library is no longer compiled into the dll to avoid incompatibilities with other mods that use this library to create UIs.

`0.9.7`
- Added multi-currency

`0.9.6`
- Fixed error when you tried to buy or delete an item that there are several purchase options in the store, now you delete or buy the one you select from the UI

`0.9.5`
- Added Stacks for products

`0.9.0`
- Gloomrot Update

`0.8.3`
- Removed debug logs to improve server performance

`0.8.2`
- Simplified core inventory

`0.8.1`

- First public version of the mod

</details>

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/userui.png?raw=true)

[Manual](https://github.com/oscarpedrero/BloodyShop/wiki/Manual)

[Known bugs](https://github.com/oscarpedrero/BloodyShop/wiki/Known-bugs)

# Important note

- At the moment it only works in the server version, the client mode with the UI is disabled.
- Removed the daily drop system, an external mod has been generated for this called [BloodyRewards](https://thunderstore.io/c/v-rising/p/Trodi/BloodyRewards/)

## Gloomrot Update

Since the update to Gloomrot the server does not have the names of the Prefabs, so the name of the Prefab has been added to the add command as a necessary parameter.

Add currency:
.shop currency add "Silver Coin" -949672483 true

New add command:
.shop add "\<NameOfProduct\>" \<PrefabGUID\> \<Currency\> \<Price\> \<Stock\> \<Stack\> \<Buff| true/false \> 

Example Item:
.shop add "Silver Currency" -949672483 1 1 1 100 1 false

Example Buff:
.shop add Cursed 1425734039 1 1 1 1 true

[Buff List by Vex](https://github.com/oscarpedrero/BloodyShop/wiki/Manual#buff-list-recomendation-by-vex-vexor-gaming)

For this same reason, it is best to manage the store from the client, which implies that you must [install the BloodyShop mod on the client](https://github.com/oscarpedrero/BloodyShop/wiki/Manual#requirements) and the server if you are a server administrator.

[Complete list of prefabs](https://vrising-website.pages.dev/items)

# Credits

This mod idea was a suggestion from [@Daavy](https://ideas.vrisingmods.com/posts/11/silver-shop) on our community idea tracker, please go vote and suggest your ideas: https://ideas.vrisingmods.com/

[V Rising Mod Community](https://discord.gg/vrisingmods) the best community of mods for V Rising

@Godwin for all the ideas you have brought to the project and testing the mod on your server.

[@Vexor Gaming](https://discord.gg/AyyenSJH) For giving me ideas and testing the mods on your server and with your community.

[@Deca](https://github.com/decaprime) for your help and the wonderful framework [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone) based in [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel)

[@Adain](https://github.com/adainrivers) for encouraging me to develop a UI to be able to use the mod from the client, for the support and for its [VRising.GameData](https://github.com/adainrivers/VRising.GameData) framework

[@Paps](https://github.com/phillipsOG) for all the help and encouragement he has given us to get this idea off the ground.

[@NopeyBoi](https://github.com/NopeyBoi) Author of the buff system that I extracted from [ChatCommands](https://github.com/NopeyBoi/ChatCommands) mod

**A special thanks to the testers and supporters of the project:**

- @Vex [Vexor Gaming](https://discord.gg/rxaTBzjuMc) as a tester and great supporter, who provided his server as a test platform!
- @SynovA [Iron Fist RPG](https://discord.gg/iron-fist-rpg) as a tester and great supporter, who provided his server as a test platform!
- @iska as a tester and great supporter!
- @JosiUwU as a supporter for manuals.
