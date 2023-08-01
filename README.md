
# BloodyShop Mod to create a store in VRising (with optional client UI)

<details>
<summary>Changelog</summary>

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

[Manual](https://github.com/oscarpedrero/BloodyShop/wiki/Manual-%E2%80%90-Gloomrot-Update)

[Known bugs](https://github.com/oscarpedrero/BloodyShop/wiki/Known-bugs)

## Gloomrot Update

Since the update to Gloomrot the server does not have the names of the Prefabs, so the name of the Prefab has been added to the add command as a necessary parameter.

New command:
.shop add "\<NameOfProduct\>" \<PrefabGUID\> \<Price\> \<Stock\>

Example:
.shop add "Silver Currency" -949672483 1 100

This name is only visible through the chat commands, if the client uses the mod he will see it in the language that the game has configured

For this same reason, it is best to manage the store from the client, which implies that you must [install the BloodyShop mod on the client](https://github.com/oscarpedrero/BloodyShop/wiki/Manual#requirements) and the server if you are a server administrator.

[Complete list of prefabs](https://discord.com/channels/978094827830915092/1117273637024714862/1117273642817044571)

# Credits

This mod idea was a suggestion from [@Daavy](https://ideas.vrisingmods.com/posts/11/silver-shop) on our community idea tracker, please go vote and suggest your ideas: https://ideas.vrisingmods.com/

[V Rising Mod Community](https://discord.gg/vrisingmods) the best community of mods for V Rising

@Godwin for all the ideas you have brought to the project and testing the mod on your server.

[@Vexor Gaming](https://discord.gg/AyyenSJH) For giving me ideas and testing the mods on your server and with your community.

[@Deca](https://github.com/decaprime) for your help and the wonderful framework [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone) based in [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel)

[@Adain](https://github.com/adainrivers) for encouraging me to develop a UI to be able to use the mod from the client, for the support and for its [VRising.GameData](https://github.com/adainrivers/VRising.GameData) framework

[@Paps](https://github.com/phillipsOG) for all the help and encouragement he has given us to get this idea off the ground.