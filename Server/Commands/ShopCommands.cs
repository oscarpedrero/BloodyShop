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
using Wetstone.API;

namespace BloodyShop.Server.Commands
{
    [CommandGroup("shop")]
    internal class ShopCommands
    {

        [Command("add", usage: "<PrefabGuid> <Price> <Stock>", description: "Add a product to the store. To know the PrefabGuid of an item you must look for the item in the following URL <#4acc45><u>https://gaming.tools/v-rising/items</u></color>", adminOnly: true)]
        public static void addItem(ChatCommandContext ctx, int item, int price, int stock)
        {
            try
            {
                var prefabGUID = new PrefabGUID(item);
                var itemModel = GameData.Items.GetPrefabById(prefabGUID);

                if (itemModel == null)
                {
                    throw ctx.Error("Invalid item type");
                }

                if (!ShareDB.getCoin(out ItemModel coin))
                {
                    throw ctx.Error("Error loading currency type");
                }

                if (!ItemsDB.addProductList(item, price, stock))
                {
                    throw ctx.Error("Invalid item type");
                }

                SaveDataToFiles.saveProductList();

                ctx.Reply(FontColorChat.Yellow($"Added item {FontColorChat.White($"{itemModel?.Name.ToString()} ({stock})")} to the store with a price of {FontColorChat.White($"{price} {coin?.Name.ToString()}")}"));
                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{itemModel?.Name.ToString()} ({stock})")} have been added to the Store for {FontColorChat.White($"{price} {coin?.Name.ToString()}")}"));
                }
                var usersOnline = GameData.Users.Online;
                foreach (var user in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user.Internals.User, msg);
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

                if (!ShareDB.getCoin(out ItemModel coin))
                {
                    throw ctx.Error("Error loading currency type");
                }

                if (quantity <= 0)
                {
                    throw ctx.Error($"The minimum purchase quantity of a product is 1");
                }

                if (!ItemsDB.SearchItemByCommand(indexPosition, out PrefabModel itemShopModel))
                {
                    throw ctx.Error("This item is not available in the store");
                }

                var finalPrice = itemShopModel.PrefabPrice * quantity;

                if (!itemShopModel.CheckStockAvailability(quantity))
                {
                    throw ctx.Error("There is not enough stock of this item");
                }

                if (!InventorySystem.verifyHaveSuficientCoins(playerCharacter, coin.PrefabGUID, finalPrice))
                {
                    throw ctx.Error("There is not enough stock of this item");
                }

                if (!InventorySystem.getCoinsFromInventory(playerCharacter, ctx.Event.User.CharacterName.ToString(), coin.PrefabName, coin.PrefabGUID, finalPrice))
                {
                    throw ctx.Error($"You need {FontColorChat.White($"{finalPrice} {coin.Name}")} in your inventory for this purchase");
                }

                if (!InventorySystem.AdditemToIneventory(ctx.Event.User.CharacterName.ToString(), new PrefabGUID(itemShopModel.PrefabGUID), quantity))
                {
                    Plugin.Logger.LogInfo($"Error buying an item User: {ctx.Event.User.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    throw ctx.Error($"An error has occurred when delivering the items, please contact an administrator");
                }

                ctx.Reply(FontColorChat.Yellow($"Transaction successful. You have purchased {FontColorChat.White($"{quantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {coin.Name}")}"));

                if (!ItemsDB.ModifyStockByCommand(indexPosition, quantity))
                {
                    Plugin.Logger.LogInfo($"Error ModifyStockByCommand: {ctx.Event.User.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    return;
                }

                SaveDataToFiles.saveProductList();
                LoadDataFromFiles.loadProductList();

                var usersOnline = GameData.Users.Online;
                foreach (var user in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user.Internals.User, msg);
                }

                if (ConfigDB.AnnounceBuyPublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{ctx.Event.User.CharacterName} has purchased {FontColorChat.White($"{quantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {coin.Name}")}"));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogInfo($"Error: {error.Message}");
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

                var usersOnline = GameData.Users.Online;
                foreach (var user in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user.Internals.User, msg);
                }

                if(ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));
                }
                

            } catch (Exception error)
            {
                Plugin.Logger.LogInfo($"Error: {error.Message}");
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

            if (!ShareDB.getCoin(out ItemModel coin))
            {
                throw ctx.Error("Error loading currency type");
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

            ctx.Reply(FontColorChat.Yellow($"To buy an object you must have in your inventory the number of {FontColorChat.White(coin.Name.ToString())} indicated by each product."));
            ctx.Reply(FontColorChat.Yellow($"Use the chat command \"{FontColorChat.White($"shop buy <NumberItem> <Quantity> ")}\""));

        }

        [Command("open", usage:"", description:"Open store", adminOnly: true)]
        public static void OpenShop(ChatCommandContext ctx)
        {
            

            ConfigDB.ShopEnabled = true;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just opened"));
            var usersOnline = GameData.Users.Online;
            var msg = new OpenSerializedMessage();
            foreach (var user in usersOnline)
            {
                ServerOpenMessageAction.Send(user.Internals.User, msg);
            }
            
        }

        [Command("close", usage:"", description: "Close store", adminOnly: true)]
        public static void CloseShop(ChatCommandContext ctx)
        {
            ConfigDB.ShopEnabled = false;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just closed"));
            var usersOnline = GameData.Users.Online;
            var msg = new CloseSerializedMessage();
            foreach (var user in usersOnline)
            {
                ServerCloseMessageAction.Send(user.Internals.User, msg);
            }
        }
    }
}
