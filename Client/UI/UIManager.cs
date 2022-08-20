using UniverseLib.UI;
using System.IO;
using UniverseLib;
using UnityEngine;
using System;
using BloodyShop.Client.Network;
using BloodyShop.Client.UI.Panels;
using BloodyShop.Client.DB;
using BloodyShop.DB;
using VRising.GameData;

namespace BloodyShop.Client.UI;

internal class UIManager
{


    internal static void Initialize()
    {
        const float startupDelay = 3f;
        UniverseLib.Config.UniverseLibConfig config = new()
        {
            Disable_EventSystem_Override = false, // or null
            Force_Unlock_Mouse = false, // or null
            Allow_UI_Selection_Outside_UIBase = true,
            Unhollowed_Modules_Folder = Path.Combine(BepInEx.Paths.BepInExRootPath, "unhollowed") // or null
        };

        Universe.Init(startupDelay, OnInitialized, LogHandler, config);


    }

    public static UIBase UiBase { get; private set; }
    public static ShopPanel ShopPanel { get; private set; }
    public static AdminPanel AdminPanel { get; private set; }
    public static MenuPanel MenuPanel { get; private set; }

    public static bool ActiveShopPanel { get;  set; }
    public static bool ActiveAdminPanel { get;  set; }
    public static int SizeOfProdutcs { get;  set; }

    static void OnInitialized()
    {
        Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        UiBase = UniversalUI.RegisterUI(unixTimestamp.ToString(), UiUpdate);
        ActiveShopPanel = false;
        ActiveAdminPanel = false;
    }

    public static void CreateMenuPanel()
    {
        MenuPanel = new MenuPanel(UiBase);
    }

    public static void OpenShopPanel()
    {
        if (ClientDB.shopOpen)
        {
            if (ActiveShopPanel)
            {
                ShopPanel?.Toggle();
            }
            else
            {
                ActiveShopPanel = true;
                ShopPanel = new ShopPanel(UiBase);
            }
        }
        
    }

    public static void OpenAdminPanel()
    {
        if (ClientDB.userModel.IsAdmin)
        {
            if (ActiveAdminPanel)
            {
                AdminPanel?.Toggle();
            }
            else
            {
                ActiveAdminPanel = true;
                AdminPanel = new AdminPanel(UiBase);
            }
        }

    }
    public static void RefreshDataPanel()
    {
        ShopPanel?.RefreshData();
        AdminPanel?.RefreshData();
        ClientListMessageAction.Send();
    }

    public static void CloseMenuPanel()
    {
        MenuPanel?.Toggle();
        ShopPanel?.Toggle();
        AdminPanel?.Toggle();
    }

    static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }

    static void LogHandler(string message, LogType type)
    {
        // ...
    }

}