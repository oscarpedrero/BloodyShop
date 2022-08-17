using UniverseLib.UI;
using GT.VRising.GameData;
using System.IO;
using UniverseLib;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UniverseLib.UI.Models;
using BloodyShop.DB;
using GT.VRising.GameData.Models;
using UniverseLib.Input;

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
            Unhollowed_Modules_Folder = Path.Combine(BepInEx.Paths.BepInExRootPath, "unhollowed") // or null
        };

        Universe.Init(startupDelay, OnInitialized, LogHandler, config);

    }

    public static UIBase UiBase { get; private set; }
    public static MainPanel MainPanel { get; private set; }

    static void OnInitialized()
    {
       
    }

    public static void createPanel()
    {
        UiBase = UniversalUI.RegisterUI(Plugin.Guid, UiUpdate);
        MainPanel = new MainPanel(UiBase);
        InputManager.BeginRebind(OnSelection, OnFinished);
        Plugin.Logger.LogInfo("Iniciamos la UI");
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

public class MainPanel : UniverseLib.UI.Panels.PanelBase 
{

    public static MainPanel Instance { get; private set; }


    /*
    public MainPanel(UIBase owner) : base(owner) { }
    public override string Name => "Shopìfy Admin";
    public override int MinWidth => 100;
    public override int MinHeight => 200;
    public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
    public override Vector2 DefaultAnchorMax => new(0.75f, 0.75f);
    public override bool CanDragAndResize => true;*/

    public override string Name => "Black Market";
    //public override UIManager.Panels PanelType => UIManager.Panels.Inspector;
    //public override bool ShouldSaveActiveState => false;

    public override int MinWidth => 530;
    public override int MinHeight => 400;
    public override Vector2 DefaultAnchorMin => new(510, 250);
    public override Vector2 DefaultAnchorMax => new(480, 250);
    public override Vector2 DefaultPosition => new(1, 1);

    public GameObject NavbarHolder;
    public Dropdown MouseInspectDropdown;
    public GameObject ContentHolder;
    public GameObject ContentProduct;
    public RectTransform ContentRect;

    public static float CurrentPanelWidth => Instance.Rect.rect.width;
    public static float CurrentPanelHeight => Instance.Rect.rect.height;

    public MainPanel(UIBase owner) : base(owner)
    {
        Instance = this;
    }

    public override void Update()
    {
        //InspectorManager.Update();
    }

    public override void OnFinishResize()
    {
        base.OnFinishResize();

        //InspectorManager.PanelWidth = this.Rect.rect.width;
        //InspectorManager.OnPanelResized(Rect.rect.width);
    }

    protected override void ConstructPanelContent()
    {


        /**/
        GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;

        // Inspect under mouse dropdown on title bar

        // add close all button to titlebar

        ButtonRef adminStoreBtn = UIFactory.CreateButton(closeHolder.gameObject, "AdminStoreBtn", "Admin",
            new Color(0.3f, 0.2f, 0.2f));
        UIFactory.SetLayoutElement(adminStoreBtn.Component.gameObject, minHeight: 25, minWidth: 80);
        adminStoreBtn.Component.transform.SetSiblingIndex(adminStoreBtn.Component.transform.GetSiblingIndex() - 1);
        ButtonRef refreshBtn = UIFactory.CreateButton(closeHolder.gameObject, "RefreshBtn", "Refresh",
            new Color(0.3f, 0.2f, 0.2f));
        UIFactory.SetLayoutElement(refreshBtn.Component.gameObject, minHeight: 25, minWidth: 80);
        refreshBtn.Component.transform.SetSiblingIndex(refreshBtn.Component.transform.GetSiblingIndex() - 1);
        //closeAllBtn.OnClick += InspectorManager.CloseAllTabs;

        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(this.ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

        this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
            new Color(0.05f, 0.05f, 0.05f));
        UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 0, preferredHeight: 0, flexibleHeight: 9999);
        NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        //this.ContentHolder = UIFactory.CreateVerticalGroup(this.ContentRoot, "ContentHolder", true, true, true, true, 0, default,
        // new Color(0.1f, 0.1f, 0.1f));
        //var items = GameData.Items.Prefabs.Take(10).Select(w => w.Name).Where(w => !string.IsNullOrEmpty(w)).OrderBy(w => w);

        var items = ItemsDB.GetProductList();

        var index = 1;
        foreach(var item in items)
        {
            if (ShareDB.getCoin(out ItemModel coin))
            {


                var _contentProduct = UIFactory.CreateHorizontalGroup(this.ContentRoot, "ContentItem", true, true, true, true, 0, default,
                new Color(0.1f, 0.1f, 0.1f));

                // IMAGE
                /*var textureViewerRoot = UIFactory.CreateVerticalGroup(_contentProduct, "TextureViewer", false, false, true, true, 2, new Vector4(5, 5, 5, 5),
                    new Color(0.1f, 0.1f, 0.1f), childAlignment: TextAnchor.UpperLeft);
                UIFactory.SetLayoutElement(textureViewerRoot, flexibleWidth: 9999, flexibleHeight: 9999);
                GameObject imageViewport = UIFactory.CreateVerticalGroup(textureViewerRoot, "ImageViewport", false, false, true, true,
                    bgColor: new(1, 1, 1, 0), childAlignment: TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(imageViewport, flexibleWidth: 9999, flexibleHeight: 9999);

                GameObject imageHolder = UIFactory.CreateUIObject("ImageHolder", imageViewport);
                var imageLayout = UIFactory.SetLayoutElement(imageHolder, 1, 1, 0, 0);

                GameObject actualImageObj = UIFactory.CreateUIObject("ActualImage", imageHolder);
                RectTransform actualRect = actualImageObj.GetComponent<RectTransform>();
                actualRect.anchorMin = new(0, 0);
                actualRect.anchorMax = new(1, 1);
                var image = actualImageObj.AddComponent<Image>();
                Sprite sprite = TextureHelper.CreateSprite(item.Internals.ManagedItemData.Icon.texture);
                image.sprite = sprite;*/

                //NUMBER ITEM
                Text indexName = UIFactory.CreateLabel(_contentProduct, "indexTxt", $"{index}", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(indexName.gameObject, minWidth: 10, minHeight: 25);
                index++;

                //NAME ITEM
                Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt", $"{item.getItemName()}", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 200, minHeight: 25);

                // PRICE ITEM
                Text itemPrice = UIFactory.CreateLabel(_contentProduct, "itemPriceTxt", $"{item.price} {coin.Name}", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 100, minHeight: 25);

                // Quantity ITEM
                Text itemQuantity = UIFactory.CreateLabel(_contentProduct, "itemQuantityTxt", $"{item.amount}", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 10, minHeight: 25);

                // BUY BTN
                ButtonRef buyBtn = UIFactory.CreateButton(_contentProduct, "buyItemBtn", "BUY",
                new Color(0.3f, 0.2f, 0.2f));
                UIFactory.SetLayoutElement(buyBtn.Component.gameObject, minHeight: 25, minWidth: 80);

                /*UIFactory.CreateDropdown(
                    _contentProduct,
                    "Weapons",
                    out var dropdown,
                    "Select a weapon",
                    14,
                    WeaponDropdownValueChanged,
                    GameData.Items.Prefabs.Select(w => w.Name).Where(w => !string.IsNullOrEmpty(w)).OrderBy(w => w).ToArray());*/




                UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 9999);
            }
        }

        //ContentRect = _contentProduct.GetComponent<RectTransform>();

        this.SetActive(true);



        
    }

    private void WeaponDropdownValueChanged(int val)
    {
        throw new NotImplementedException();
    }

    // override other methods as desired
}

