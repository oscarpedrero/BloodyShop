using BloodyShop.Client.DB;
using BloodyShop.Client.UI;
using BloodyShop.Network.Messages;
using BloodyShop.Server.DB;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientCloseMessageAction
    {

        public static void Received(CloseSerializedMessage msg)
        {

            UIManager.HideShopPanel();

            ClientDB.shopOpen = false;

            if (ClientDB.IsAdmin)
            {
                UIManager.AdminMenuPanel.closeShop();
            } else
            {
                UIManager.MenuPanel.closeShop();
            }

            //Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] CloseSerializedMessage");
        }

        public static void Send(CloseSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new CloseSerializedMessage();
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] CloseSerializedMessage");
        }

    }
}
