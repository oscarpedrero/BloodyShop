using UniverseLib.UI;
using System.IO;
using UniverseLib;
using UnityEngine;
using System;
using BloodyShop.Client.Network;
using BloodyShop.Client.DB;
using BloodyShop.Client.UI.Panels.Admin;
using BloodyShop.Client.UI.Panels.User;

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
    public static AddItemPanel AddItemPanel { get; private set; }
    public static DeleteItemPanel DeleteItemPanel { get; private set; }
    public static MenuPanel MenuPanel { get; private set; }
    public static AdminMenuPanel AdminMenuPanel { get; private set; }

    public static bool ActiveShopPanel { get;  set; }
    public static bool ActiveAddItemPanel { get;  set; }
    public static bool ActiveDeleteItemPanel { get;  set; }
    public static bool ActiveAdminMenuPanel { get;  set; }
    public static int SizeOfProdutcs { get;  set; }

    static void OnInitialized()
    {
        Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        UiBase = UniversalUI.RegisterUI(unixTimestamp.ToString(), UiUpdate);
        ActiveShopPanel = false;
        ActiveDeleteItemPanel = false;
        ActiveAdminMenuPanel = false;
    }

    public static void CreateMenuPanel()
    {
        if (ClientDB.userModel.IsAdmin)
        {
            AdminMenuPanel = new AdminMenuPanel(UiBase);
        } else
        {
            MenuPanel = new MenuPanel(UiBase);
        }
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

    public static void OpenAddItemPanel()
    {
        if (ClientDB.userModel.IsAdmin)
        {
            if (ActiveAddItemPanel)
            {
                AddItemPanel?.Toggle();
            }
            else
            {
                ActiveAddItemPanel = true;
                AddItemPanel = new AddItemPanel(UiBase);
            }
        }

    }

    public static void OpenDeletePanel()
    {
        if (ClientDB.userModel.IsAdmin)
        {
            if (ActiveDeleteItemPanel)
            {
                DeleteItemPanel?.Toggle();
            }
            else
            {
                ActiveDeleteItemPanel = true;
                DeleteItemPanel = new DeleteItemPanel(UiBase);
            }
        }

    }

    public static void OpenAdminMenuPanel()
    {
        if (ClientDB.userModel.IsAdmin)
        {
            if (ActiveAdminMenuPanel)
            {
                AdminMenuPanel?.Toggle();
            }
            else
            {
                ActiveAdminMenuPanel = true;
                AdminMenuPanel = new AdminMenuPanel(UiBase);
            }
        }

    }
    public static void RefreshDataPanel()
    {
        ShopPanel?.RefreshData();
        DeleteItemPanel?.RefreshData();
        ClientListMessageAction.Send();
    }

    public static void CloseMenuPanel()
    {
        MenuPanel?.Toggle();
        ShopPanel?.Toggle();
        DeleteItemPanel?.Toggle();
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