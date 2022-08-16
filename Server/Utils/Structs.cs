using ProjectM.Network;
using Unity.Entities;

namespace BloodyShop.Server.Utils
{

    public struct Float2
    {
        public float x { get; set; }
        public float y { get; set; }
        public Float2(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    public struct VChatEvent
    {
        public Entity SenderUserEntity { get; set; }
        public Entity SenderCharacterEntity { get; set; }
        public string Message { get; set; }
        public ChatMessageType Type { get; set; }
        public User User { get; set; }

        public VChatEvent(Entity senderUserEntity, Entity senderCharacterEntity, string message, ChatMessageType type, User user)
        {
            SenderUserEntity = senderUserEntity;
            SenderCharacterEntity = senderCharacterEntity;
            Message = message;
            Type = type;
            User = user;
        }
    }

    public class Context
    {
        public string Prefix { get; set; }
        public VChatEvent Event { get; set; }
        public string[] Args { get; set; }
        public EntityManager EntityManager { get; set; }

        public Context(string prefix, VChatEvent ev, string[] args)
        {
            Prefix = prefix;
            Event = ev;
            Args = args;

            EntityManager = Plugin.Server.EntityManager;
        }
    }
}
