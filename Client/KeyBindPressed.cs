using BloodyShop.Client.Network;
using BloodyShop.Client.Patch;
using BloodyShop.Client.UI;
using System;

namespace BloodyShop.Client
{
    public class KeyBindPressed
    {
        internal static void OnKeyPressedOpenPanel(KeyBindFunction keybindFunction)
        {
            switch (keybindFunction)
            {
                case KeyBindFunction.ToggleUI:
                    if (ClientMod.UIInit)
                    {
                        UIManager.MainPanel?.Toggle();
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
