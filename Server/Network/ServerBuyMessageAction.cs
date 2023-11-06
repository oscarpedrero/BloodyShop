using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using VampireCommandFramework;
using System;
using BloodyShop.Utils;
using ProjectM;
using VRising.GameData;
using BloodyShop.Server.DB;
using BloodyShop.DB.Models;
using VRising.GameData.Models;
using Unity.Entities;
using BloodyShop.Server.Core;

namespace BloodyShop.Server.Network
{
    public class ServerBuyMessageAction
    {
        private static CurrencyModel currency;

        public static void Received(FromCharacter fromCharacter, BuySerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            var buyID = Int32.Parse(msg.ItemIndex);
            var quantity = Int32.Parse(msg.Quantity);
            var name = msg.Name;

            BuyItem(user, fromCharacter.Character, name, buyID, quantity);

        }

        public static void BuyItem(User user, Entity playerCharacter, string itemName, int indexPosition, int quantity)
        {

            try
            {
                
                var userModel = GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());

                if (!ConfigDB.ShopEnabled)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"{FontColorChat.White($"{ConfigDB.StoreName}")} is closed"));
                    return;
                }

                if (quantity <= 0)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"The minimum purchase quantity of a product is 1"));
                    return;
                }

                if (!ItemsDB.SearchItemByCommand(indexPosition, out PrefabModel itemShopModel))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("This item is not available in the store"));
                    return;
                }

                currency = ShareDB.getCurrency(itemShopModel.currency);

                if (currency == null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error loading currency type"));
                    return;
                }

                if (itemShopModel.PrefabStack <= 0)
                {
                    itemShopModel.PrefabStack = 1;
                }


                var finalPrice = itemShopModel.PrefabPrice * quantity;

                if (!itemShopModel.CheckStockAvailability(quantity))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("There is not enough stock of this item"));
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user, msg);
                    return;
                }

                var currencyItemModel = GameData.Items.GetPrefabById(new PrefabGUID(currency.guid));

                if (!InventorySystem.verifyHaveSuficientPrefabsInInventory(user.CharacterName.ToString(), currencyItemModel.PrefabGUID, finalPrice))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"You need {FontColorChat.White($"{finalPrice} {currency.name}")} in your inventory for this purchase"));
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user, msg);
                    return;
                }

                
                if (!InventorySystem.getPrefabFromInventory(user.CharacterName.ToString(), currencyItemModel.PrefabGUID, finalPrice))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"You need {FontColorChat.White($"{finalPrice} {currency.name}")} in your inventory for this purchase"));
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(user, msg);
                    return;
                }

                var finalQuantity = itemShopModel.PrefabStack * quantity;

                if (itemShopModel.isBuff)
                {
                    BuffSystem.BuffPlayer(playerCharacter, userModel.Entity, new PrefabGUID(itemShopModel.PrefabGUID), 0, true);
                }
                else
                {    
                
                    if (!InventorySystem.AdditemToInventory(user.CharacterName.ToString(), new PrefabGUID(itemShopModel.PrefabGUID), finalQuantity))
                    {
                        Plugin.Logger.LogError($"Error buying an item User: {user.CharacterName.ToString()} Item: {itemShopModel.PrefabStack}x {itemName} Quantity: {quantity} TotalPrice: {finalPrice}");
                        ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"An error has occurred when delivering the items, please contact an administrator"));
                        return;
                    }

                }

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Yellow($"Transaction successful. You have purchased {FontColorChat.White($"{finalQuantity}x {itemName}")} for a total of  {FontColorChat.White($"{finalPrice} {currency.name}")}"));

                if (!ItemsDB.ModifyStockByCommand(indexPosition, quantity))
                {
                    Plugin.Logger.LogError($"Error ModifyStockByCommand: {user.CharacterName.ToString()} Item: {itemName} Quantity: {quantity} TotalPrice: {finalPrice}");
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
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send(userValue, msg);
                    }
                }

                if (ConfigDB.AnnounceBuyPublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{user.CharacterName} has purchased {FontColorChat.White($"{finalQuantity}x {itemName}")} for a total of  {FontColorChat.White($"{finalPrice} {currency.name}")}"));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"Error: {error.Message}"));
            }
        }

        public static void Send(User fromCharacter, BuySerializedMessage msg)
        {
           return;
        }

    }
}
