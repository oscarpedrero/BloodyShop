using Stunlock.Network;
using Unity.Collections;
using Bloodstone.API;

namespace BloodyShop.Network.Messages
{
    public class AddSerializedMessage : VNetworkMessage
    {

        public string PrefabGUID;

        public string Price;

        public string Stock;

        public string Name;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public AddSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            PrefabGUID = reader.ReadString(Allocator.Temp);
            Price = reader.ReadString(Allocator.Temp);
            Stock = reader.ReadString(Allocator.Temp);
            Name = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(ref NetBufferOut writer)
        {
            writer.Write(PrefabGUID);
            writer.Write(Price);
            writer.Write(Stock);
            writer.Write(Name);
        }

    }
}
