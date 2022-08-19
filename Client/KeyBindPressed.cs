using BloodyShop.Client.Network;
using BloodyShop.Client.Patch;
using BloodyShop.Client.UI;
using System;
using UnityEngine;

namespace BloodyShop.Client
{
    public class KeyBindPressed
    {
        internal static void OnKeyPressedOpenPanel(KeyBindFunction keybindFunction)
        {
            switch (keybindFunction)
            {
                case KeyBindFunction.ToggleShopUI:
                    if (ClientMod.UIInit)
                    {
                        UIManager.MenuPanel?.Toggle();
                        UIManager.ShopPanel?.Toggle();
                        UIManager.AdminPanel?.Toggle();
                    } else
                    {
                        ClientListMessageAction.Send();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keybindFunction), keybindFunction, null);
            }
        }
    }
}
