using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
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

        public static void Received(FromCharacter fromCharacter, BuySerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            var buyID = Int32.Parse(msg.ItemIndex);
            var quantity = Int32.Parse(msg.Quantity);

            BuyItem(user, fromCharacter.Character, buyID, quantity);

        }

        public static void BuyItem(User user, Entity playerCharacter, int indexPosition, int quantity)
        {

            try
            {

                if (!ConfigDB.ShopEnabled)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"{FontColorChat.White($"{ConfigDB.StoreName}")} is closed"));
                    return;
                }

                if (!ShareDB.getCoin(out ItemModel coin))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error loading currency type"));
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

                var finalPrice = itemShopModel.PrefabPrice * quantity;

                if (!itemShopModel.CheckStockAvailability(quantity))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("There is not enough stock of this item"));
                    return;
                }

                if (!InventorySystem.verifyHaveSuficientPrefabsInInventory(user.CharacterName.ToString(), coin.PrefabGUID, finalPrice))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"You need {FontColorChat.White($"{finalPrice} {coin.Name}")} in your inventory for this purchase"));
                    return;
                }

                if (!InventorySystem.getPrefabFromInventory(user.CharacterName.ToString(), coin.PrefabGUID, finalPrice))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"You need {FontColorChat.White($"{finalPrice} {coin.Name}")} in your inventory for this purchase"));
                    return;
                }

                if (!InventorySystem.AdditemToInventory(user.CharacterName.ToString(), new PrefabGUID(itemShopModel.PrefabGUID), quantity))
                {
                    Plugin.Logger.LogInfo($"Error buying an item User: {user.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"An error has occurred when delivering the items, please contact an administrator"));
                    return;
                }

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Yellow($"Transaction successful. You have purchased {FontColorChat.White($"{quantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {coin.Name}")}"));

                if (!ItemsDB.ModifyStockByCommand(indexPosition, quantity))
                {
                    Plugin.Logger.LogInfo($"Error ModifyStockByCommand: {user.CharacterName.ToString()} Item: {itemShopModel.PrefabName} Quantity: {quantity} TotalPrice: {finalPrice}");
                    return;
                }

                SaveDataToFiles.saveProductList();
                LoadDataFromFiles.loadProductList();

                var usersOnline = GameData.Users.Online;
                foreach (var userOnline in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(userOnline.Internals.User, msg);
                }

                if (ConfigDB.AnnounceBuyPublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{user.CharacterName} has purchased {FontColorChat.White($"{quantity}x {itemShopModel.PrefabName}")} for a total of  {FontColorChat.White($"{finalPrice} {coin.Name}")}"));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogInfo($"Error: {error.Message}");
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"Error: {error.Message}"));
            }
        }



        public static void Send(User fromCharacter, BuySerializedMessage msg)
        {
           return;
        }

    }
}
