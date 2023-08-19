using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using BloodyShop.Server.Core;
using BloodyShop.Server.DB;
using BloodyShop.Server.Network;
using BloodyShop.Utils;
using ProjectM;
using System;
using VampireCommandFramework;
using VRising.GameData;
using VRising.GameData.Models;
using Bloodstone.API;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Data;
using BloodyShop.Server.Systems;

namespace BloodyShop.Server.Commands
{
    [CommandGroup("shop")]
    internal class ShopCommands
    {
        public static CurrencyModel currency { get; private set; }
        public static List<CurrencyModel> currencies { get; private set; }

        [Command("currency add", usage: "\"<Name>\" <PrefabGuid>", description: "Add a currency to the store. To know the PrefabGuid of an item you must look for the item in the following URL <#4acc45><u>https://gaming.tools/v-rising/items</u></color>", adminOnly: true)]
        public static void AddCurrency(ChatCommandContext ctx, string name, int item)
        {

            var prefabGUID = new PrefabGUID(item);
            var itemModel = GameData.Items.GetPrefabById(prefabGUID);

            if (itemModel == null)
            {
                throw ctx.Error("Invalid item type");
            }

            if (!ShareDB.addCurrencyList(name, item))
            {
                throw ctx.Error("Invalid item type");
            }

            SaveDataToFiles.saveCurrenciesList();

            ctx.Reply(FontColorChat.Yellow($"Added currency {FontColorChat.White($"{name}")} to the store"));

            var userWithUI = UserUI.GetUsersWithUI();
            foreach (var userUI in userWithUI)
            {
                var userValue = userUI.Value;
                if (userValue.IsConnected && userValue.IsAdmin)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                }
            }

        }

        [Command("currency list", usage: "", description: "List of products available to buy in the store", adminOnly: true)]
        public static void ListCurrency(ChatCommandContext ctx)
        {

            if (!ConfigDB.ShopEnabled)
            {
                throw ctx.Error(FontColorChat.Yellow($"{FontColorChat.White($"{ConfigDB.StoreName}")} is closed"));
            }

            var currencyList = ShareDB.GetCurrencyListMessage();

            if (currencyList.Count <= 0)
            {
                throw ctx.Error("No currency available in the store");
            }

            foreach (string item in currencyList)
            {
                ctx.Reply(item);
            }

        }

        [Command("currency remove", shortHand: "crm", usage: "<NumberItem>", description: "Delete a currency from the store", adminOnly: true)]
        public static void RemoveCurrency(ChatCommandContext ctx, int index)
        {

            try
            {
                if (ShareDB.currenciesList.Count == 1)
                {
                    throw ctx.Error(FontColorChat.Yellow($"Do not remove all currency from the store."));
                }
                if (!ShareDB.SearchCurrencyByCommand(index, out CurrencyModel currecyModel))
                {
                    throw ctx.Error(FontColorChat.Yellow($"Currency removed error."));
                }
                if (!ShareDB.RemoveCurrencyyByCommand(index))
                {
                    throw ctx.Error(FontColorChat.Yellow($"Item {FontColorChat.White($"{currecyModel.name}")} removed error."));
                }

                SaveDataToFiles.saveCurrenciesList();
                LoadDataFromFiles.loadCurrencies();

                ctx.Reply(FontColorChat.Yellow($"Currency {FontColorChat.White($"{currecyModel.name}")} removed successful."));

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if (userValue.IsConnected && userValue.IsAdmin)
                    {
                        // TODO SEND CUrrencys
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                    }
                }

            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                throw ctx.Error($"Error: {error.Message}");
            }

        }

        [Command
        (
            "add", 
            usage: "\"<Name>\" <PrefabGuid> <Currency> <Price> <Stock> <Stack>", 
            description: "Add a product to the store. To know the PrefabGuid of an item you must look for the item in the following URL <#4acc45><u>https://gaming.tools/v-rising/items</u></color>", 
            adminOnly: true)
        ]
        public static void AddItem(ChatCommandContext ctx, string name, int item, int currencyId, int price, int stock, int stack=1)
        {
            try
            {
                var prefabGUID = new PrefabGUID(item);
                var itemModel = GameData.Items.GetPrefabById(prefabGUID);

                if (itemModel == null)
                {
                    throw ctx.Error("Invalid item type");
                }

                currencies = ShareDB.getCurrencyList();

                currency = currencies.FirstOrDefault(currency => currency.id == currencyId);

                if (currency == null)
                {
                    throw ctx.Error("Error loading currency type");
                }

                if (!ItemsDB.addProductList(item, price, stock, name, stack))
                {
                    throw ctx.Error("Invalid item type");
                }

                SaveDataToFiles.saveProductList();

                ctx.Reply(FontColorChat.Yellow($"Added item {FontColorChat.White($"{stack}x {name} ({stock})")} to the store with a price of {FontColorChat.White($"{price} {currency?.name.ToString()}")}"));
                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{stack}x {name} ({stock})")} have been added to the Store for {FontColorChat.White($"{price} {currency?.name.ToString()}")}"));
                }

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if (userValue.IsConnected)
                    {
                        // TODO SEND CUrrencys
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                    }
                }
               
                return;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError(error.Message);
                throw ctx.Error("Error saving the item in the store ");
            }
        }

        [Command("buy", usage: "<NumberItem> <Quantity> ", description: "Buy an object from the shop", adminOnly: false)]
        public static void BuyItem(ChatCommandContext ctx, int indexPosition, int quantity)
        {

            try
            {
                var playerCharacter = ctx.Event.SenderCharacterEntity;

                if (!ConfigDB.ShopEnabled)
                {
                    throw ctx.Error(FontColorChat.Yellow($"{FontColorChat.White($"{ConfigDB.StoreName}")} is closed"));
                }

                if (quantity <= 0)
                {
                    throw ctx.Error($"The minimum purchase quantity of a product is 1");
                }

                if (!ItemsDB.SearchItemByCommand(indexPosition, out PrefabModel itemShopModel))
                {
                    throw ctx.Error("This item is not available in the store");
                }

                currency = ShareDB.getCurrency(itemShopModel.currency);

                if (currency == null)
                {
                    throw ctx.Error("Error loading currency type");
                }

                var finalPrice = itemShopModel.PrefabPrice * quantity;

                if (!itemShopModel.CheckStockAvailability(quantity))
                {
                    throw ctx.Error("There is not enough stock of this item");
                }

                var currencyItemModel = GameData.Items.GetPrefabById(new PrefabGUID(currency.guid));

                if (!InventorySystem.verifyHaveSuficientPrefabsInInventory(ctx.Event.User.CharacterName.ToString(), currencyItemModel.PrefabGUID, finalPrice))
                {
                    throw ctx.Error($"You need {FontColorChat.White($"{finalPrice} {currency.name}")} in your inventory for this purchase");
                }

                if (!InventorySystem.getPrefabFromInventory(ctx.Event.User.CharacterName.ToString(), currencyItemModel.PrefabGUID, finalPrice))
                {
                    throw ctx.Error($"You need {FontColorChat.White($"{finalPrice} {currency.name}")} in your inventory for this purchase");
                }

                var finalQuantity = itemShopModel.PrefabStack * quantity;

                if (!InventorySystem.AdditemToInventory(ctx.Event.User.CharacterName.ToString(), new PrefabGUID(itemShopModel.PrefabGUID), finalQuantity))
                {
                    Plugin.Logger.LogError($"Error buying an item User: {ctx.Event.User.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    throw ctx.Error($"An error has occurred when delivering the items, please contact an administrator");
                }

                ctx.Reply(FontColorChat.Yellow($"Transaction successful. You have purchased {FontColorChat.White($"{quantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {currency.name}")}"));

                if (!ItemsDB.ModifyStockByCommand(indexPosition, quantity))
                {
                    Plugin.Logger.LogError($"Error ModifyStockByCommand: {ctx.Event.User.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    return;
                }

                SaveDataToFiles.saveProductList();
                LoadDataFromFiles.loadProductList();

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if (userValue.IsConnected)
                    {
                        // TODO SEND CUrrencys
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                    }
                }

                if (ConfigDB.AnnounceBuyPublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{ctx.Event.User.CharacterName} has purchased {FontColorChat.White($"{finalQuantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {currency.name}")}"));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                throw ctx.Error($"Error: {error.Message}");
            }
        }

        [Command("remove", shortHand: "rm", usage: "<NumberItem>", description: "Delete a product from the store", adminOnly: true)]
        public static void removeItemFromShop(ChatCommandContext ctx, int index)
        {
            try
            {
                if (!ItemsDB.SearchItemByCommand(index, out PrefabModel itemShopModel))
                {
                    throw ctx.Error(FontColorChat.Yellow($"Item removed error."));
                }
                if (!ItemsDB.RemoveItemByCommand(index))
                {
                    throw ctx.Error(FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed error."));
                }

                SaveDataToFiles.saveProductList();
                LoadDataFromFiles.loadProductList();

                ctx.Reply(FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if (userValue.IsConnected)
                    {
                        // TODO SEND CUrrencys
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                    }
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));
                }
                

            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                throw ctx.Error($"Error: {error.Message}");
            }
        }

        [Command("list", usage: "", description: "List of products available to buy in the store", adminOnly: false)]
        public static void list(ChatCommandContext ctx)
        {
            if (!ConfigDB.ShopEnabled)
            {
                throw ctx.Error(FontColorChat.Yellow($"{FontColorChat.White($"{ConfigDB.StoreName}")} is closed"));
            }

            var listProduct = ItemsDB.GetProductListMessage();

            if (listProduct.Count <= 0)
            {
                throw ctx.Error("No products available in the store");
            }

            foreach (string item in listProduct)
            {
                ctx.Reply(item);
            }

            ctx.Reply(FontColorChat.Yellow($"To buy an object you must have in your inventory the number of currency indicated by each product."));
            ctx.Reply(FontColorChat.Yellow($"Use the chat command \"{FontColorChat.White($"shop buy <NumberItem> <Quantity> ")}\""));

        }

        [Command("open", usage:"", description:"Open store", adminOnly: true)]
        public static void OpenShop(ChatCommandContext ctx)
        {
            

            ConfigDB.ShopEnabled = true;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just opened"));
            var userWithUI = UserUI.GetUsersWithUI();
            var msg = new OpenSerializedMessage();
            foreach (var userUI in userWithUI)
            {
                var userValue = userUI.Value;
                if (userValue.IsConnected)
                {
                    // TODO SEND CUrrencys
                    ServerOpenMessageAction.Send((ProjectM.Network.User)userValue, msg);
                }
            }
            
        }

        [Command("close", usage:"", description: "Close store", adminOnly: true)]
        public static void CloseShop(ChatCommandContext ctx)
        {
            ConfigDB.ShopEnabled = false;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just closed"));
            var userWithUI = UserUI.GetUsersWithUI();
            var msg = new CloseSerializedMessage();
            foreach (var userUI in userWithUI)
            {
                var userValue = userUI.Value;
                if (userValue.IsConnected)
                {
                    // TODO SEND CUrrencys
                    ServerCloseMessageAction.Send((ProjectM.Network.User)userValue, msg);
                }
            }
        }
    }
}
