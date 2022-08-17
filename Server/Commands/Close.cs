using ProjectM;
using BloodyShop.Server.DB;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Text;

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

                ConfigDB.setShopEnabled(false);
                ServerChatUtils.SendSystemMessageToAllClients(ctx.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.getStoreName()} ")} just closed"));
            }
            else
            {
                Output.InvalidCommand(ctx.Event);
            }
        }

    }
}
