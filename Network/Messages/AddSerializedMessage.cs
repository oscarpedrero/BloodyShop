using Stunlock.Network;
using Unity.Collections;
using Wetstone.API;

namespace BloodyShop.Network.Messages
{
    public class AddSerializedMessage : VNetworkMessage
    {

        public string PrefabGUID;

        public string Price;

        public string Quantity;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public AddSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            PrefabGUID = reader.ReadString(Allocator.Temp);
            Price = reader.ReadString(Allocator.Temp);
            Quantity = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(NetBufferOut writer)
        {
            writer.Write(PrefabGUID);
            writer.Write(Price);
            writer.Write(Quantity);
        }
    }
}
