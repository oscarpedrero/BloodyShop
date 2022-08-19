using VRising.GameData;
using ProjectM;
using BloodyShop.Server.DB;
using BloodyShop.DB;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using System;
using VRising.GameData.Models;
using BloodyShop.Server.Network;

namespace BloodyShop.Server.Commands
{
    [Command("add", Usage = "add <PrefabGuid> <Price> <Amount>", Description = "Add a product to the store. To know the PrefabGuid of an item you must look for the item in the following URL <#4acc45><u>https://gaming.tools/v-rising/items</u></color>")]
    public static class Add
    {
        public static object ItemsData { get; private set; }

        public static void Initialize(Context ctx)
        {
            addItem(ctx);
        }

        public static void addItem(Context ctx)
        {
            if (ctx.Event.User.IsAdmin)
            {
                var args = ctx.Args;

                if (args.Length < 3 || args.Length > 3)
                {
                    Output.InvalidArguments(ctx);
                    return;
                }

                try
                {
                    var prefabGUID = new PrefabGUID(Int32.Parse(args[0]));
                    var itemModel = GameData.Items.GetPrefabById(prefabGUID);

                    if (itemModel != null)
                    {
                        if (ShareDB.getCoin(out ItemModel coin))
                        {

                            ItemsDB.addProductList(Int32.Parse(args[0]), Int32.Parse(args[1]), Int32.Parse(args[2]));
                            SaveDataToFiles.saveProductList();

                            Output.SendSystemMessage(ctx, FontColorChat.Yellow($"Added item {FontColorChat.White($"{itemModel?.Name.ToString()} ({args[2]})")} to the store with a price of {FontColorChat.White($"{args[1]} {coin?.Name.ToString()}")}"));
                            if (ConfigDB.getShopEnabled())
                            {
                                ServerChatUtils.SendSystemMessageToAllClients(ctx.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{args[2]}x {itemModel?.Name.ToString()}")} have been added to the Store for {FontColorChat.White($"{args[1]} {coin?.Name.ToString()}")}"));
                                var usersOnline = GameData.Users.Online;
                                foreach (var user in usersOnline)
                                {
                                    var msg = ServerListMessageAction.createMsg(user.Internals.User);
                                    ServerListMessageAction.Send(user.Internals.User, msg);
                                }
                            }
                            return;
                        }
                        else
                        {
                            Output.CustomErrorMessage(ctx, "Error loading currency type");
                        }

                    }
                    else
                    {
                        Output.CustomErrorMessage(ctx, "Invalid item type");
                    }


                }
                catch (Exception error)
                {
                    Plugin.Logger.LogError(error.Message);
                    Output.CustomErrorMessage(ctx, "Error saving the item in the store ");
                    return;
                }

            }
            else
            {
                Output.InvalidCommand(ctx.Event);
            }
        }
    }
}
