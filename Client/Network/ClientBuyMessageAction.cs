using BloodyShop.Client.UI;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientBuyMessageAction
    {

        public static void Received(BuySerializedMessage msg)
        {
            //Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] BuySerializedMessage {msg.ItemIndex}");
        }

        public static void Send(BuySerializedMessage msg = null)
        {

            if(msg == null)
            {
                msg = new BuySerializedMessage();
                msg.ItemIndex = "1";
            }

            VNetwork.SendToServer(msg);
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] BuySerializedMessage - {msg.ItemIndex}");
        }

    }
}
