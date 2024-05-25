using UniverseLib.UI;
using System.IO;
using UniverseLib;
using UnityEngine;
using System;
//using BloodyShop.Client.Network;
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
    //public static AddItemPanel AddItemPanel { get; private set; }
    public static PanelConfig panelCOnfig { get; private set; }
    //public static DeleteItemPanel DeleteItemPanel { get; private set; }
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
        CreatePanelCOnfigPanel();
    }

    public static void showAllPanels()
    {
        ShowMenuPanel();
        ShowShopPanel();
        ShowPanelConfigPanel();
    }

    public static void HideAllPanels()
    {
        HideMenuPanel();
        HideShopPanel();
        HidePanelConfigPanel();
    }

    public static void DestroyAllPanels()
    {
        HideMenuPanel();
        HideShopPanel();
        HidePanelConfigPanel();
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

    public static void CreatePanelCOnfigPanel()
    {
        if (ClientDB.IsAdmin)
        {
            panelCOnfig = new PanelConfig(UiBase);
            panelCOnfig.SetActive(false);
        }
    }

    public static void ShowPanelConfigPanel()
    {
        if (ClientDB.IsAdmin)
        {
            panelCOnfig.SetActive(true);
        }
    }

    public static void HidePanelConfigPanel()
    {
        if (ClientDB.IsAdmin)
        {
            panelCOnfig.SetActive(false);
        }
    }

    public static void DestroyPanelConfigPanel()
    {
        if (ClientDB.IsAdmin)
        {
            panelCOnfig.Destroy();
        }
    }

    public static void RefreshDataPanel()
    {
        ShopPanel.RefreshData();
        if (ClientDB.IsAdmin)
        {
            var panelDeleteItem = panelCOnfig.GetActivePanel();
            panelDeleteItem.RefreshData();
            var panelDeleteCurrency = panelCOnfig.GetCurrencyPanel();
            panelDeleteCurrency.RefreshData();
        }
        //ClientListMessageAction.Send();
        
    }

    public static void RefreshDataAddPanel()
    {
        var panelDeleteItem = panelCOnfig.GetActivePanel();
        panelDeleteItem.RefreshData();
        var panelDeleteCurrency = panelCOnfig.GetCurrencyPanel();
        panelDeleteCurrency.RefreshData();
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