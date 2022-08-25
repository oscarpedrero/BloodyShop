using BloodyShop.Client.DB;
using BloodyShop.Client.Network;
using BloodyShop.Client.Patch;
using BloodyShop.Client.UI;
using System;
using UnityEngine;

namespace BloodyShop.Client
{
    public class KeyBindPressed
    {
        public static bool UIActive = true;
        internal static void OnKeyPressedOpenPanel(KeyBindFunction keybindFunction)
        {
            switch (keybindFunction)
            {
                case KeyBindFunction.ToggleShopUI:
                    if (ClientMod.UIInit)
                    {
                        if (UIActive)
                        {
                            UIActive = false;
                            UIManager.HideAllPanels();
                        } else
                        {
                            UIActive = true;
                            UIManager.ShowMenuPanel();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keybindFunction), keybindFunction, null);
            }
        }
    }
}
