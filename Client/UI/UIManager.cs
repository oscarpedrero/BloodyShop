using UniverseLib.UI;
using VRising.GameData;
using System.IO;
using UniverseLib;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UniverseLib.UI.Models;
using BloodyShop.DB;
using VRising.GameData.Models;
using UniverseLib.Input;
using UnityEngine.EventSystems;
using BloodyShop.Client.Network;
using BloodyShop.Network.Messages;
using BloodyShop.Client.UI.Panels;

namespace BloodyShop.Client.UI;

internal class UIManager
{


    internal static void Initialize()
    {
        const float startupDelay = 3f;
        UniverseLib.Config.UniverseLibConfig config = new()
        {
            Disable_EventSystem_Override = false, // or null
            Force_Unlock_Mouse = true, // or null
            Allow_UI_Selection_Outside_UIBase = true,
            Unhollowed_Modules_Folder = Path.Combine(BepInEx.Paths.BepInExRootPath, "unhollowed") // or null
        };

        Universe.Init(startupDelay, OnInitialized, LogHandler, config);

    }

    public static UIBase UiBase { get; private set; }
    public static MainPanel MainPanel { get; private set; }
    public static AdminPanel AdminPanel { get; private set; }

    static void OnInitialized()
    {
        Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        UiBase = UniversalUI.RegisterUI(unixTimestamp.ToString(), UiUpdate);
    }

    public static void createPanel()
    {
        MainPanel = new MainPanel(UiBase);
        if (AdminPanel.active)
        {
            AdminPanel = new AdminPanel(UiBase);
        }
    }
    public static void removePanel()
    {
        if (ClientMod.UIInit)
        {
            ClientMod.UIInit = false;
            MainPanel.Destroy();
            if (AdminPanel.active)
            {
                AdminPanel.Destroy();
            }
            ClientListMessageAction.Send();
        }
    }

    public static void showAdminPanel()
    {
        if (AdminPanel.active)
        {
            AdminPanel?.Toggle();
        } else
        {
            AdminPanel = new AdminPanel(UiBase);
        }
        
    }

    static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }

    static void LogHandler(string message, LogType type)
    {
        // ...
    }

    static void OnBeginRebindPressed()
    {
        InputManager.BeginRebind(OnSelection, OnFinished);
    }

    static void OnEndRebindPressed()
    {
        InputManager.EndRebind();
    }

    static void OnSelection(KeyCode pressed)
    {
        if (InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.KeypadEnter))
        {
            Plugin.Logger.LogInfo("HIT ENTER");
            return;
        }
    }

    static void OnFinished(KeyCode? bound)
    {
        // The bound key may be null, indicating the user didn't press anything.
    }

}