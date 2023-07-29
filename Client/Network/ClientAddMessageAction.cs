using BloodyShop.Network.Messages;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientAddMessageAction
    {

        public static void Received(AddSerializedMessage msg)
        {
            //Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] CloseSerializedMessage");
        }

        public static void Send(AddSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new AddSerializedMessage();
                msg.PrefabGUID = "0";
                msg.Price = "0";
                msg.Stock = "0";
                msg.Name = "";
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] CloseSerializedMessage");
        }

    }
}
