using Stunlock.Network;
using Wetstone.API;

namespace BloodyShop.Network.Messages
{
    public class CloseSerializedMessage : VNetworkMessage
    {

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public CloseSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {

        }

        // Write your contents to the writer.
        public void Serialize(NetBufferOut writer)
        {

        }
    }
}
