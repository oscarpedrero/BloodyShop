﻿using Stunlock.Network;
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
        public string CoinGUID;
        public string ShopName;
        public string CharacterName;
        public string ShopOpen;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public ListSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            ItemsJson = reader.ReadString(Allocator.Temp);
            CoinGUID = reader.ReadString(Allocator.Temp);
            ShopName = reader.ReadString(Allocator.Temp);
            CharacterName = reader.ReadString(Allocator.Temp);
            ShopOpen = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(NetBufferOut writer)
        {
            writer.Write(ItemsJson);
            writer.Write(CoinGUID);
            writer.Write(ShopName);
            writer.Write(CharacterName);
            writer.Write(ShopOpen);
        }
    }
}
