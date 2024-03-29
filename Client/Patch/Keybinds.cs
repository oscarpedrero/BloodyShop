﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Bloodstone;
using Bloodstone.API;

namespace BloodyShop.Client.Patch
{
    internal delegate void OnKeyPressedEventHandler(KeyBindFunction keybindFunction);

    internal enum KeyBindFunction
    {
        ToggleShopUI
    }

    internal static class KeyBinds
    {
        internal static event OnKeyPressedEventHandler OnKeyPressed;

        private static Keybinding _toggleUiKeyBind;
        private static KeyBindBehaviour _keybindBehavior;

        internal static void Initialize()
        {
            if (VWorld.IsClient)
            {

                _toggleUiKeyBind = KeybindManager.Register(new()
                {
                    Id = Plugin.Guid + "ToggleUI",
                    Category = Plugin.Name,
                    Name = "Toggle Shop UI",
                    DefaultKeybinding = KeyCode.F11,
                });
                _keybindBehavior = Plugin.Instance.AddComponent<KeyBindBehaviour>();
            }
        }

        internal static void Uninitialize()
        {
            if (_keybindBehavior != null)
            {
                UnityEngine.Object.Destroy(_keybindBehavior);
            }
        }

        private class KeyBindBehaviour : MonoBehaviour
        {
            private void Update()
            {
                if (_toggleUiKeyBind.IsPressed)
                {
                    OnKeyPressed?.Invoke(KeyBindFunction.ToggleShopUI);
                }
            }
        }
    }
}
