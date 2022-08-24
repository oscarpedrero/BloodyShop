using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Server.DB;
using BloodyShop.Server.Network;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using VRising.GameData;

namespace BloodyShop.Server.Commands
{
    [Command("remove, rm", Usage = "remove <NumberItem>", Description = "Delete a product from the store")]
    public static class Remove
    {
        public static void Initialize(Context ctx)
        {
            removeItemFromShop(ctx);
        }

        public static void removeItemFromShop(Context ctx)
        {
            if (ctx.Event.User.IsAdmin)
            {
                var args = ctx.Args;

                if (args.Length < 1 || args.Length > 1)
                {
                    Output.InvalidArguments(ctx);
                    return;
                }


                try
                {
                    var index = Int32.Parse(args[0]);
                    if (ItemsDB.SearchItemByCommand(index, out PrefabModel itemShopModel))
                    {
                        if (ItemsDB.RemoveItemByCommand(index))
                        {
                            SaveDataToFiles.saveProductList();
                            LoadDataFromFiles.loadProductList();
                            Output.SendSystemMessage(ctx, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));

                            var usersOnline = GameData.Users.Online;
                            foreach (var user in usersOnline)
                            {
                                var msg = ServerListMessageAction.createMsg();
                                ServerListMessageAction.Send(user.Internals.User, msg);
                            }

                        }
                        else
                        {
                            Output.SendSystemMessage(ctx, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed error."));
                        }
                    }
                }
                catch (Exception error)
                {
                    Plugin.Logger.LogInfo($"Error: {error.Message}");
                    Output.SendSystemMessage(ctx, FontColorChat.Yellow($"Item removed error."));
                }
            }
        }
    }
}
