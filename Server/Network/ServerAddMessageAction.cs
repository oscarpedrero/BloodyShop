using ProjectM.Network;
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

            var prefabGUID = msg.PrefabGUID;
            var price = msg.Price;
            var quantity = msg.Quantity;

            Plugin.Logger.LogInfo($"{prefix} add {prefabGUID} {price} {quantity}");

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} add {prefabGUID} {price} {quantity}", new ChatMessageType(), user);

            string[] args = new string[] { prefabGUID, price, quantity };

            var ctx = new Context(prefix, vchatEvent, args);

            Add.addItem(ctx);

        }

        public static void Send(User fromCharacter, AddSerializedMessage msg)
        {
           return;
        }

    }
}
