/*using Stunlock.Network;
using Unity.Collections;
using Bloodstone.API;

namespace BloodyShop.Network.Messages
{
    public class AddCurrencySerializedMessage : VNetworkMessage
    {

        public string CurrencyGUID;

        public string Name;
        public string Drop;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public AddCurrencySerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            CurrencyGUID = reader.ReadString(Allocator.Temp);
            Name = reader.ReadString(Allocator.Temp);
            Drop = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(ref NetBufferOut writer)
        {
            writer.Write(CurrencyGUID);
            writer.Write(Name);
            writer.Write(Drop);
        }

    }
}
*/