using ProjectM;
using BloodyShop.Server.DB;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using BloodyShop.Server.Network;
using VRising.GameData;
using BloodyShop.Network.Messages;

namespace BloodyShop.Server.Commands
{
    [Command("open", Usage = "open", Description = "Open store")]
    public static class Open
    {
        public static void Initialize(Context ctx)
        {

            OpenShop(ctx);

        }

        public static void OpenShop(Context ctx)
        {
            if (ctx.Event.User.IsAdmin)
            {
                var args = ctx.Args;

                if (args.Length < 0 || args.Length > 0)
                {
                    Output.InvalidArguments(ctx);
                    return;
                }

                ConfigDB.setShopEnabled(true);
                ServerChatUtils.SendSystemMessageToAllClients(ctx.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.getStoreName()} ")} just opened"));
                var usersOnline = GameData.Users.Online;
                var msg = new OpenSerializedMessage();
                foreach (var user in usersOnline)
                {
                    ServerOpenMessageAction.Send(user.Internals.User, msg);
                }
            }
            else
            {
                Output.InvalidCommand(ctx.Event);
            }
        }
    }
}
