# BloodyShop Shop mod for VRising

It's a beta, don't use this version until I've made it to an official release.

# Server Admin

## Configuration
We will start with the configuration part.

Once the mod is installed, it's time for configuration:

For this we will go to the folder that we have defined within BepInEx for the configuration files and there we will find the configuration file of the mod called `BloodyShop.cfg` that we will explain:

```
## Plugin GUID: BloodyShop

[ConfigShop]

## Enable Shop
# Setting type: Boolean
# Default value: true
enabled = true

## Store's name. This name will also serve as a prefix for the command, that is, if you put Black Market, for example, the system will parse the name, remove space and pass it to lowercase, so the command will be !blackmarket
# Setting type: String
# Default value: Bloody Shop
name = Bloody Shop

## Item that will be used as currency within the service, by default they are silver coins, if you want to change the item you must include the GUID of said object that you can get from https://gaming.tools/v-rising/items
# Setting type: Int32
# Default value: -949672483
coinGUID = -949672483

[DropSystem]

## Enable Drop System
# Setting type: Boolean
# Default value: true
enabled = true

## Percent chance that an NPC will drop the type of currency from the shop
# Setting type: Int32
# Default value: 5
minPercentageDropNpc = 5

## Percentage increase for every rank of 10 levels of the NPC
# Setting type: Int32
# Default value: 5
IncrementPercentageDropEveryTenLevelsNpc = 5

## Minimum currency an NPC can drop
# Setting type: Int32
# Default value: 1
DropdNpcCoinsMin = 1

## Maximum currency an NPC can drop
# Setting type: Int32
# Default value: 5
DropNpcCoinsMax = 5

## Maximum number of currency that a user can get per day by NPC death
# Setting type: Int32
# Default value: 5
MaxCoinsPerDayPerPlayerNpc = 5

## Percent chance that an VBlood will drop the type of currency from the shop
# Setting type: Int32
# Default value: 20
minPercentageDropVBlood = 20

## Percentage increase for every rank of 10 levels of the VBlood
# Setting type: Int32
# Default value: 5
IncrementPercentageDropEveryTenLevelsVBlood = 5

## Minimum currency an VBlood can drop
# Setting type: Int32
# Default value: 10
DropVBloodCoinsMin = 10

## Maximum currency an VBlood can drop
# Setting type: Int32
# Default value: 20
DropVBloodCoinsMax = 20

## Maximum number of currency that a user can get per day by VBlood death
# Setting type: Int32
# Default value: 20
MaxCoinsPerDayPerPlayerVBlood = 20

## Percent chance that victory in a PVP duel will drop the type of currency in the store
# Setting type: Int32
# Default value: 100
minPercentageDropPvp = 100

## Percentage increase for every rank of 10 levels of the Player killed in pvp duel
# Setting type: Int32
# Default value: 5
IncrementPercentageDropEveryTenLevelsPvp = 5

## Minimum currency can drop victory in PVP
# Setting type: Int32
# Default value: 15
DropPvpCoinsMin = 15

## Maximum currency can drop victory in PVP
# Setting type: Int32
# Default value: 20
DropPvpCoinsMax = 20

## Maximum number of currency that a user can get per day by victory in PVP
# Setting type: Int32
# Default value: 20
MaxCoinsPerDayPerPlayerPvp = 20
```

|SECTION|PARAM| DESCRIPTION                                                     | DEFAULT
|----------------|-------------------------------|-----------------------------------------------------------------|-----------------------------|
|ConfigShop|`enabled `            | Define if the store is open or closed | true
|ConfigShop|`name`| Store's name. This name will also serve as a prefix for the command, that is, if you put Black Market, for example, the system will parse the name, remove space and pass it to lowercase, so the command will be !blackmarket                 |Bloody Shop
|ConfigShop|`coinGUID `            | Item that will be used as currency within the service, by default they are silver coins, if you want to change the item you must include the GUID of said object that you can get from https://gaming.tools/v-rising/items              | [-949672483](https://gaming.tools/v-rising/items/item_ingredient_silvercoin)
|DropSystem|`enabled`            | Enable Drop System  | true
|DropSystem|`minPercentageDropNpc`            | Percent chance that an NPC will drop the type of currency from the shop | 5
|DropSystem|`IncrementPercentageDropEveryTenLevelsNpc`            |  Percentage increase for every rank of 10 levels of the NPC| 5
|DropSystem|`DropdNpcCoinsMin`            |  Minimum currency an NPC can drop| 5
|DropSystem|`DropNpcCoinsMax`            |  Maximum currency an NPC can drop| 5
|DropSystem|`MaxCoinsPerDayPerPlayerNpc`            |  Maximum number of currency that a user can get per day by NPC death| 5
|DropSystem|`minPercentageDropVBlood`            |  Percent chance that an VBlood will drop the type of currency from the shop| 20
|DropSystem|`IncrementPercentageDropEveryTenLevelsVBlood`            |  Percentage increase for every rank of 10 levels of the VBlood| 1
|DropSystem|`DropVBloodCoinsMin`            |  Minimum currency an VBlood can drop| 10
|DropSystem|`DropVBloodCoinsMax`            |  Maximum currency an VBlood can drop| 20
|DropSystem|`MaxCoinsPerDayPerPlayerVBlood`            |  Maximum number of currency that a user can get per day by VBlood death| 20
|DropSystem|`minPercentageDropPvp`            |  Percent chance that victory in a PVP duel will drop the type of currency in the store| 100
|DropSystem|`IncrementPercentageDropEveryTenLevelsPvp`            |  Percentage increase for every rank of 10 levels of the Player killed in pvp duel| 5
|DropSystem|`DropPvpCoinsMin`            |  Minimum currency can drop victory in PVP| 15
|DropSystem|`DropPvpCoinsMax`            |  Maximum currency can drop victory in PVP| 20
|DropSystem|`MaxCoinsPerDayPerPlayerPvp`            |  Maximum number of currency that a user can get per day by victory in PVP| 20

## Admin commands

There are currently four commands for admins:

| COMMAND                                          |DESCRIPTION
|--------------------------------------------------|-------------------------------|
| `!bloodyshop open`                                   | Command to open the store         
| `!bloodyshop close`                 | Command to open the store.
| `!bloodyshop add <PrefabGUID> <Price> <Stock>`   | Command to add an item to the store. To get the PrefabGUID you must visit the web [https://gaming.tools/v-rising/items](https://gaming.tools/v-rising/items) and get the PrefabGUID Value as shown in the image below
| `!bloodyshop remove <PrefabGUID>` | Command to remove an item from the store. To get the PrefabGUID you must visit the web [https://gaming.tools/v-rising/items](https://gaming.tools/v-rising/items) and get the PrefabGUID Value as shown in the image below

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/prefabGUID.JPG?raw=true)

## Admin UI

To have the UI in the game and be able to manage the store as admin you must install the mod in your client.

To do this, copy the .BloodyShop.dll and Wetstone.dll into the BepInEx plugins folder.

### Configuration

The only configuration that exists for the mod is the key to open or close the UI.

For this we will go to Configuration -> Controls and at the end of the list you will see the option BLOODYSHOP -> Toggle Shop UI where you must configure the key you want for this action.

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/configKey.png?raw=true)

Once this is done, connect to the server where you have the mod installed.

### UI
When you enter the game, press the key that you have configured to show the UI (F11 by default) and the mod's navigation bar will appear (centered above) with the following options:

| BUTTON                                          |DESCRIPTION
|--------------------------------------------------|-------------------------------|
| `Shop Opened / Closed`                                   | Button to open the store UI to buy an item
| `Add Item`                 | Button to open the admin UI to add items to the store
| `Delete Item`                 | Button to open the admin UI to remove items from the store
| `Open\Close Store`                 | Button to open or close the store within the game, this will imply that users will be able to buy or not items in the store

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/navbar.JPG?raw=true)

### Add Item

The first thing we are going to see is the administration tools.

We will click on the Add Item button to open the UI and add an object

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/adminaddui.png?raw=true)

The first thing we see is a bar where we can write the name of the item that we want to include in the store.  Once we start writing the store, it will go looking for the products that contain the text that we are writing in the search bar.
 
Once we have the product that we want to add to the store, we only have to fill in the price of the object and the stock that we want it to have in the store and click on the Add Item button.

> if we want it to be infinite we will have to enter a value of -1

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/adminadduisearch.png?raw=true)

### Delete Item

We will now move on to the UI for deleting objects.

To do this, we will click on the "Delete Item" button on the navigation bar and the UI will open to delete objects.

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/admindeluisearch.png?raw=true)

In this UI we can search for a store item or browse through the store pages until we find the desired item.

Once we have the object that we want to delete, it is as simple as clicking on the "Delete" button to delete said object from the store.

### Open / Close Store

The next thing we will do is try to open or close the store.

This option is used so that when the administrators want to close the store for any reason (or open it), they click on the "Close Store" button on the navigation bar and the store and the windows of the store users will immediately be closed.

Likewise, the button on the Navigation bar will also be deactivated to open the store UI.

Shop Open NavBar
![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/navbar.JPG?raw=true)

Shop Closed NavBar
![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/navarclosestore.png?raw=true)

# Server Player

The user's navigation bar is different from that of the server administrators.

The client will only have a button on the navigation bar to open the store UI to be able to buy, as long as the store is open.

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/navaruser.png?raw=true)

We press the "Shop Opened" button and the store UI opens where we can buy the products that are available.

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/userui.png?raw=true)

We have several ways to find products:

- Through the search engine that we have in the header of the UI
- Through pagination and the view of the UI itself.

In each line of the objects we can see the stock, the name of the object with its corresponding icon, the price of each object and the "BUY" button

> In case an object has infinite stock it will appear instead of a number "-"

![alt text](https://github.com/oscarpedrero/BloodyShop/blob/master/Images/productlist.png?raw=true)

To buy an object we will only have to hit the "BUY" button that contains each object that we want to buy.


## Drop System

We have included a drop system for the store currency to be able to control the economy of the server.

This system can be activated or not depending on whether the material we want to use as currency drops very frequently or not.

The system works in such a way that we will define a percentage of opportunity for the store's currency to drop each time the following events occur:

- Death of an npc by a player
- Death of a VBlood by a player
- Every time someone wins a PVP duel

For this we have included several parameters that are configured through the mod configuration file on the server. For each of those three events there are several values ​​that we can configure:

- Percentage of chance of falling when the event jumps
- Percentage of drop chance increase every 10 levels.
- Minimum number of store coins that can drop in each event
- Maximum number of store coins that can drop in each event
- Maximum number of coins a player can get through this drop system.

Let's take an example for an NPC drop system:

- We want to have a 10% probability that an npc, when killed, drops a number of coins from the store of a minimum of 1 and a maximum of 5
- And that the increase every 10 levels of that NPC is 2%
- With a maximum of 40 coins per day for each player.

To make this type of configuration we would have to put the following parameters in the server file:

```
## Percent chance that an NPC will drop the type of currency from the shop
# Setting type: Int32
#Default value: 5
minPercentageDropNpc = 10

## Percentage increase for every rank of 10 levels of the NPC
# Setting type: Int32
#Default value: 5
IncrementPercentageDropEveryTenLevelsNpc = 2

## Minimum currency an NPC can drop
# Setting type: Int32
#Default value: 1
DropdNpcCoinsMin = 1

## Maximum currency an NPC can drop
# Setting type: Int32
#Default value: 5
DropNpcCoinsMax = 5

## Maximum number of currency that a user can get per day by NPC death
# Setting type: Int32
#Default value: 5
MaxCoinsPerDayPerPlayerNpc = 40
```
This is what happened in the different cases of the drop system on the death of an NPC:

- If the NPC is level from 1 to 10, his percentage chance of having a drop of coins from the earth is 10% with a maximum of 1 coin and a maximum of 5.
- If the NPC is level from 11 to 20, his percentage chance of having a drop of coins from the earth is 12% with a maximum of 1 coin and a maximum of 5.
- If the NPC is level from 21 to 30, his percentage chance of having a drop of coins from the earth is 14% with a maximum of 1 coin and a maximum of 5.
- If the NPC is level 31 to 40, his percentage chance of having a drop of coins from the earth is 16% with a maximum of 1 coin and a maximum of 5.
- ...      And so we would continue up to the level that is indicated for each NPC until we reach the limit per player per day, which in that case would not have any chance of getting drops from the store.

This same configuration can be customized for the three drop events that we mentioned above.

# Known bugs

- [ ] If you disconnect from the server and try to connect again to the same server or another server, the game closes directly


