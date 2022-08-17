using BloodyShop.Network.Messages;
using Wetstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientCloseMessageAction
    {

        public static void Received(CloseSerializedMessage msg)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] CloseSerializedMessage");
        }

        public static void Send(CloseSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new CloseSerializedMessage();
            }

            VNetwork.SendToServer(msg);
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] CloseSerializedMessage");
        }

    }
}
