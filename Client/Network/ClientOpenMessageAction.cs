/*using BloodyShop.Client.DB;
using BloodyShop.Client.UI;
using BloodyShop.Network.Messages;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientOpenMessageAction
    {

        public static void Received(OpenSerializedMessage msg)
        {

            if (ClientDB.IsAdmin)
            {
                UIManager.AdminMenuPanel.openShop();
            }
            else
            {
                UIManager.MenuPanel.openShop();
            }

            UIManager.RefreshDataPanel();

           // Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] OpenSerializedMessage");
        }

        public static void Send(OpenSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new OpenSerializedMessage();
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] OpenSerializedMessage");
        }

    }
}*/
