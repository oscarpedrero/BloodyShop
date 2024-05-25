/*using BloodyShop.Network.Messages;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientAddCurrencyMessageAction
    {

        public static void Received(AddCurrencySerializedMessage msg)
        {
            //Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] CloseSerializedMessage");
        }

        public static void Send(AddCurrencySerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new AddCurrencySerializedMessage();
                msg.CurrencyGUID = "0";
                msg.Name = "Unknow";
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] CloseSerializedMessage");
        }

    }
}
*/