using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Server.DB;
using BloodyShop.DB;
using BloodyShop.Utils;
using VRising.GameData.Models;

namespace BloodyShop.Server.Commands
{
    [Command("list", Usage = "list", Description = "List of products available to buy in the store")]
    internal class List
    {
        public static void Initialize(Context ctx)
        {
            if (ConfigDB.getShopEnabled())
            {
                if (ShareDB.getCoin(out ItemModel coin))
                {
                    var listProduct = ItemsDB.GetProductListMessage();
                    if (listProduct.Count > 0)
                    {
                        foreach (string item in listProduct)
                        {
                            Output.SendSystemMessage(ctx, item);
                        }
                        Output.CustomErrorMessage(ctx, FontColorChat.Yellow($"To buy an object you must have in your inventory the number of {FontColorChat.White(coin?.Name.ToString())} indicated by each product."));
                        Output.CustomErrorMessage(ctx, FontColorChat.Yellow($"Use the chat command \"{FontColorChat.White($"{ChatSystem.Prefix} buy <NumberItem> <Quantity> ")}\""));
                    }
                    else
                    {
                        Output.CustomErrorMessage(ctx, "No products available in the store");
                    }
                }
                else
                {
                    Output.CustomErrorMessage(ctx, "Error loading currency type");
                }
            } else
            {
                Output.CustomErrorMessage(ctx, FontColorChat.Yellow($"{FontColorChat.White($"{ConfigDB.getStoreName()}")} is closed"));
            }
            

        }

        
    }
}
