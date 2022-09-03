using BloodyShop.DB;
using BloodyShop.Server.DB;
using BloodyShop.Server.Network;
using BloodyShop.Utils;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Text;
using VampireCommandFramework;
using VRising.GameData;
using VRising.GameData.Models;
using Wetstone.API;

namespace BloodyShop.Server.Commands
{
    [CommandGroup("shop")]
    internal class ShopCommands
    {
        public static object ItemsData { get; private set; }

        [Command("add", usage: "add <PrefabGuid> <Price> <Stock>", description: "Add a product to the store. To know the PrefabGuid of an item you must look for the item in the following URL <#4acc45><u>https://gaming.tools/v-rising/items</u></color>", adminOnly: true)]
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

                ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{itemModel?.Name.ToString()} ({stock})")} have been added to the Store for {FontColorChat.White($"{price} {coin?.Name.ToString()}")}"));
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
    }
}
