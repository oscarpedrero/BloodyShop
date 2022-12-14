using Stunlock.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Wetstone.API;

namespace BloodyShop.Network.Messages
{
    public class ListSerializedMessage : VNetworkMessage
    {
        public string ItemsJson;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public ListSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            ItemsJson = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(NetBufferOut writer)
        {
            writer.Write(ItemsJson);
        }
    }
}
