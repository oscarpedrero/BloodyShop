using BloodyShop.Client.DB;
using BloodyShop.Client.UI;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using VRising.GameData;
using Bloodstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientCurrencyListMessageAction
    {

        public static void Received(ListCurrencySerializedMessage msg)
        {
           // Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] ListSerializedMessage {msg.ItemsJson}");

            var currencyList = JsonSerializer.Deserialize<List<CurrencyModel>>(msg.CurrencyJson);

            ShareDB.setCurrencyList(currencyList);

            if (ClientDB.IsAdmin)
            {
                var panel = UIManager.panelCOnfig.GetCurrencyPanel();
                panel.RefreshData();
                panel.CreateListProductsLayout();
            }
            
        }

        public static void Send(ListCurrencySerializedMessage msg = null)
        {
            //Plugin.Logger.LogInfo($"[CLIENT] [SEND] ListSerializedMessage");
            if (msg == null)
            {
                msg = new ListCurrencySerializedMessage();
                msg.CurrencyJson = "list";
            }

            VNetwork.SendToServer(msg);
            
        }

    }
}
