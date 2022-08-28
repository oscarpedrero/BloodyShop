using ProjectM;
using BloodyShop.Server.DB;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using VRising.GameData;
using BloodyShop.Server.Network;
using BloodyShop.Network.Messages;

namespace BloodyShop.Server.Commands
{
    [Command("close", Usage = "close", Description = "Close store")]
    public static class Close
    {
        public static void Initialize(Context ctx)
        {
            
            CloseShop(ctx);

        }

        public static void CloseShop(Context ctx)
        {
            if (ctx.Event.User.IsAdmin)
            {
                var args = ctx.Args;

                if (args.Length < 0 || args.Length > 0)
                {
                    Output.InvalidArguments(ctx);
                    return;
                }

                ConfigDB.ShopEnabled = false;
                ServerChatUtils.SendSystemMessageToAllClients(ctx.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just closed"));
                var usersOnline = GameData.Users.Online;
                var msg = new CloseSerializedMessage();
                foreach(var user in usersOnline)
                {
                    ServerCloseMessageAction.Send(user.Internals.User, msg);
                }
            }
            else
            {
                Output.InvalidCommand(ctx.Event);
            }
        }

    }
}
