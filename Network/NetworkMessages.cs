using ProjectM.Network;
using BloodyShop.Client.Network;
using BloodyShop.Network.Messages;
using Wetstone.API;

namespace BloodyShop.Server.Network
{
    public class NetworkMessages
    {

        public static void RegisterMessage()
        {
            Plugin.Logger.LogInfo($"RegisterBiDirectional Messages");

            VNetworkRegistry.RegisterBiDirectional<ListSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    ClientListMessageAction.Received(msg);
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);
                    ServerListMessageAction.Received(user, msg);
                }
            );
        }

    }
}
