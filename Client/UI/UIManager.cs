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
            Unhollowed_Modules_Folder = Path.Combine(BepInEx.Paths.BepInExRootPath, "interop") // or null
        };

        Universe.Init(startupDelay, OnInitialized, LogHandler, config);


    }

    public static UIBase UiBase { get; private set; }
    public static ShopPanel ShopPanel { get; private set; }
    public static AddItemPanel AddItemPanel { get; private set; }
    public static DeleteItemPanel DeleteItemPanel { get; private set; }
    public static MenuPanel MenuPanel { get; private set; }
    public static AdminMenuPanel AdminMenuPanel { get; private set; }

    public static int SizeOfProdutcs { get;  set; }

    static void OnInitialized()
    {
        Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        UiBase = UniversalUI.RegisterUI(unixTimestamp.ToString(), UiUpdate);
    }

    public static void CreateAllPanels()
    {
        CreateMenuPanel();
        CreateShopPanel();
        CreateAddItemPanel();
        CreateDeletePanel();
    }

    public static void showAllPanels()
    {
        ShowMenuPanel();
        ShowShopPanel();
        ShowAddItemPanel();
        ShowDeletePanel();
    }

    public static void HideAllPanels()
    {
        HideMenuPanel();
        HideShopPanel();
        HideAddItemPanel();
        HideDeletePanel();
    }

    public static void DestroyAllPanels()
    {
        HideMenuPanel();
        HideShopPanel();
        HideAddItemPanel();
        HideDeletePanel();
    }

    public static void CreateMenuPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AdminMenuPanel = new AdminMenuPanel(UiBase);
            AdminMenuPanel.SetActive(true);
        } else
        {
            MenuPanel = new MenuPanel(UiBase);
            MenuPanel.SetActive(true);
        }
    }

    public static void ShowMenuPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AdminMenuPanel.SetActive(true);
        }
        else
        {
            MenuPanel.SetActive(true);
        }
    }

    public static void HideMenuPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AdminMenuPanel.SetActive(false);
        } else
        {
            MenuPanel.SetActive(false);
        }
    }

    public static void DestroyMenuPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AdminMenuPanel.Destroy();
        } else
        {
            MenuPanel.Destroy();
        }
    }

    public static void CreateShopPanel()
    {
        ShopPanel = new ShopPanel(UiBase);
        ShopPanel.SetActive(false);
    }

    public static void ShowShopPanel()
    {
        if (ClientDB.shopOpen)
        {
            ShopPanel.SetActive(true);
        }
    }

    public static void HideShopPanel()
    {
        ShopPanel.SetActive(false);
    }

    public static void DestroyShopPanel()
    {
        ShopPanel.Destroy();
    }

    public static void CreateAddItemPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AddItemPanel = new AddItemPanel(UiBase);
            AddItemPanel.SetActive(false);
        }
    }

    public static void ShowAddItemPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AddItemPanel.SetActive(true);
        }
    }

    public static void HideAddItemPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AddItemPanel.SetActive(false);
        }
    }

    public static void DestroyAddItemPanel()
    {
        if (ClientDB.IsAdmin)
        {
            AddItemPanel.Destroy();
        }
    }

    public static void CreateDeletePanel()
    {
        if (ClientDB.IsAdmin)
        {
            DeleteItemPanel = new DeleteItemPanel(UiBase);
            DeleteItemPanel.SetActive(false);
        }
    }

    public static void ShowDeletePanel()
    {
        if (ClientDB.IsAdmin)
        {
            DeleteItemPanel.SetActive(true);
        }
    }

    public static void HideDeletePanel()
    {
        if (ClientDB.IsAdmin)
        {
            DeleteItemPanel.SetActive(false);
        }
    }

    public static void DetroyDeletePanel()
    {
        if (ClientDB.IsAdmin)
        {
            DeleteItemPanel.Destroy();
        }
    }

    public static void RefreshDataPanel()
    {
        ShopPanel.RefreshData();
        if (ClientDB.IsAdmin)
        {
            DeleteItemPanel.RefreshData();
        }
        ClientListMessageAction.Send();
    }

    public static void RefreshDataAddPanel()
    {
        AddItemPanel?.RefreshData();
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