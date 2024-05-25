/*using Stunlock.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Bloodstone.API;

namespace BloodyShop.Network.Messages
{
    public class DeleteCurrencySerializedMessage : VNetworkMessage
    {
        public string Currency;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public DeleteCurrencySerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            Currency = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(ref NetBufferOut writer)
        {
            writer.Write(Currency);
        }
    }
}*/
