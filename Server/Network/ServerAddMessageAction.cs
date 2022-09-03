﻿using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.Utils;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using System;

namespace BloodyShop.Server.Network
{
    public class ServerAddMessageAction
    {

        public static void Received(FromCharacter fromCharacter, AddSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] AddSerializedMessage {user.CharacterName}");
            
            var prefix = ChatSystem.GetPrefix();

            var prefabGUID = int.Parse(msg.PrefabGUID);
            var price = int.Parse(msg.Price);
            var stock = int.Parse(msg.Stock);

            if(Int32.Parse(stock) <= 0)
            {
                stock = "-1";
            }

            Plugin.Logger.LogInfo($"{prefix} add {prefabGUID} {price} {stock}");

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} add {prefabGUID} {price} {stock}", new ChatMessageType(), user);

            string[] args = new string[] {  };

            var ctx = new Context(prefix, vchatEvent, args);

            Add.addItem(ctx, prefabGUID, price, stock);

        }

        public static void Send(User fromCharacter, AddSerializedMessage msg)
        {
           return;
        }

    }
}
