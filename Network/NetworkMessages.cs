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
            //Plugin.Logger.LogInfo($"RegisterBiDirectional Messages");

            VNetworkRegistry.RegisterBiDirectional<ConfigSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    ClientConfigMessageAction.Received(msg);
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);
                    ServerConfigMessageAction.Received(user, msg);
                }
            );

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

            VNetworkRegistry.RegisterBiDirectional<BuySerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    ServerBuyMessageAction.Received(fromCharacter, msg);
                }
            );

            VNetworkRegistry.RegisterBiDirectional<DeleteSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    ServerDeleteMessageAction.Received(fromCharacter, msg);
                }
            );

            VNetworkRegistry.RegisterBiDirectional<OpenSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    ClientOpenMessageAction.Received(msg);
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    ServerOpenMessageAction.Received(fromCharacter, msg);
                }
            );

            VNetworkRegistry.RegisterBiDirectional<CloseSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    ClientCloseMessageAction.Received(msg);
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    ServerCloseMessageAction.Received(fromCharacter, msg);
                }
            );

            VNetworkRegistry.RegisterBiDirectional<AddSerializedMessage>(
                // invoked when the server sends a message to the client
                msg =>
                {
                    
                },

                // invoked when a client sends a message to the server
                (fromCharacter, msg) =>
                {
                    ServerAddMessageAction.Received(fromCharacter, msg);
                }
            );
        }

        public static void UnregisterMessages()
        {
            VNetworkRegistry.Unregister<ConfigSerializedMessage>();
            VNetworkRegistry.Unregister<ListSerializedMessage>();
            VNetworkRegistry.Unregister<BuySerializedMessage>();
            VNetworkRegistry.Unregister<DeleteSerializedMessage>();
            VNetworkRegistry.Unregister<OpenSerializedMessage>();
            VNetworkRegistry.Unregister<CloseSerializedMessage>();
            VNetworkRegistry.Unregister<AddSerializedMessage>();
        }

    }
}
