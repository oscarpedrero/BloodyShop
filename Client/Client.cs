using Unity.Entities;

namespace BloodyShop.Client
{
    public class ClientMod
    {

        public static bool UIInit { get; set; }

        public static void ClientEvents_OnGameDataInitialized(World world)
        {
            UIInit = false;
            
        }
    }
}
