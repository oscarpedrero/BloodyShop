using BloodyShop.Client.UI;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Wetstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientDeleteMessageAction
    {

        public static void Received(DeleteSerializedMessage msg)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] DeleteSerializedMessage {msg.Item}");
        }

        public static void Send(DeleteSerializedMessage msg = null)
        {

            if (msg == null)
            {
                msg = new DeleteSerializedMessage();
                msg.Item = "1";
            }

            VNetwork.SendToServer(msg);
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] DeleteSerializedMessage - {msg.Item}");
        }

    }
}
