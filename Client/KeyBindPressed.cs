using BloodyShop.Client.Network;
using BloodyShop.Client.Patch;
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
                    //UIManager.MainPanel?.Toggle();
                    ClientListMessageAction.Send();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keybindFunction), keybindFunction, null);
            }
        }
    }
}
