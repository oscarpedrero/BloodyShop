/*using Stunlock.Network;
using Unity.Collections;
using Bloodstone.API;

namespace BloodyShop.Network.Messages
{
    public class ConfigSerializedMessage : VNetworkMessage
    {
        public string ItemsJson;
        public string CurrenciesJson;
        public string ShopName;
        public string ShopOpen;
        public string isAdmin;

        // You need to implement an empty constructor for when your message is
        // received but not yet serialized.
        public ConfigSerializedMessage() { }

        // Read your contents from the reader.
        public void Deserialize(NetBufferIn reader)
        {
            ItemsJson = reader.ReadString(Allocator.Temp);
            CurrenciesJson = reader.ReadString(Allocator.Temp);
            ShopName = reader.ReadString(Allocator.Temp);
            ShopOpen = reader.ReadString(Allocator.Temp);
            isAdmin = reader.ReadString(Allocator.Temp);
        }

        // Write your contents to the writer.
        public void Serialize(ref NetBufferOut writer)
        {
            writer.Write(ItemsJson);
            writer.Write(CurrenciesJson);
            writer.Write(ShopName);
            writer.Write(ShopOpen);
            writer.Write(isAdmin);
        }
    }
}
*/