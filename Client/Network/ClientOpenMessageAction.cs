using BloodyShop.Client.DB;
using BloodyShop.Client.UI;
using BloodyShop.Network.Messages;
using Wetstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientOpenMessageAction
    {

        public static void Received(OpenSerializedMessage msg)
        {

            UIManager.RefreshDataPanel();

            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] OpenSerializedMessage");
        }

        public static void Send(OpenSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new OpenSerializedMessage();
            }

            VNetwork.SendToServer(msg);
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] OpenSerializedMessage");
        }

    }
}
