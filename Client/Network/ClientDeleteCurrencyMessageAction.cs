/*using BloodyShop.Client.UI;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientDeleteCurrencyMessageAction
    {

        public static void Received(DeleteCurrencySerializedMessage msg)
        {
            //Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] DeleteSerializedMessage {msg.Item}");
        }

        public static void Send(DeleteCurrencySerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new DeleteCurrencySerializedMessage();
                msg.Currency = "1";
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] DeleteSerializedMessage - {msg.Item}");
        }

    }
}*/
