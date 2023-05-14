using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using System.Linq;

public class itemEditor : EditorWindow
{
    private ItemDetailList_SO dataBase;
    private List<ItemDetails> ItemDetailsList = new List<ItemDetails>();

    private VisualTreeAsset itemRowTemplate;

    private ListView ItemListView;

    private ScrollView ItemDetailInfo;
    private ItemDetails activeItem;

    private VisualElement previewIcon;
    private Sprite defaultIcon; 

    [MenuItem("M_menu/ItemEditor")]
    public static void ShowExample()
    {
        itemEditor wnd = GetWindow<itemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        //import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //初始化
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemRowTemplete.uxml");

        ItemListView = root.Q<VisualElement>("BaseContainer").Q<VisualElement>("ItemList").Q<ListView>("ListView");

        ItemDetailInfo = root.Q<ScrollView>("ItemDetails");

        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_Game.png");

        //获取增加、减少的Button
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteItemClicked;

        LoadDatabase();

        GenerateListView();


    }
    public void OnAddItemClicked()
    {
        ItemDetails itemDetails = new ItemDetails();
        itemDetails.ItemID = 1000 + ItemDetailsList.Count + 1;
        ItemDetailsList.Add(itemDetails);
        ItemListView.Rebuild();
    }

    public void OnDeleteItemClicked()
    {
        if (activeItem != null)
        {
            ItemDetailsList.Remove(activeItem);
        }
        ItemListView.Rebuild();
        ItemDetailInfo.visible = false;

    }

    private void LoadDatabase()
    {
        var dataBaseStringArray = AssetDatabase.FindAssets("ItemDetailList_SO");

        if (dataBaseStringArray.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(dataBaseStringArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path,typeof(ItemDetailList_SO)) as ItemDetailList_SO;
        }
        EditorUtility.SetDirty(dataBase);
        ItemDetailsList = dataBase.ItemDetailsList;
    }



    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < ItemDetailsList.Count)
            {
                if (ItemDetailsList[i].itemIcon != null)
                {
                    e.Q<VisualElement>("Icon").style.backgroundImage = ItemDetailsList[i].itemIcon.texture;
                }

                e.Q<Label>("Name").text = ItemDetailsList[i] == null ? "NO ITEM" : ItemDetailsList[i].ItemName;

            }
        };
        ItemListView.fixedItemHeight = 60f;
        
        ItemListView.makeItem = makeItem;
        ItemListView.bindItem = bindItem;
        ItemListView.itemsSource = ItemDetailsList;


//对ItemDetail的操作
        ItemDetailInfo.visible = false;

        //在ListView中得到当前选择的Item
        ItemListView.onSelectionChange += OnListSelectionChange;
    }
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GenerateDetails();
        ItemDetailInfo.visible = true;
    }





    private void GenerateDetails()
    {
        //实现在UIBuilder直接更改数据
        ItemDetailInfo.MarkDirtyRepaint();

       //ItemID
        ItemDetailInfo.Q<IntegerField>("ItemID").value = activeItem.ItemID;
        ItemDetailInfo.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemID = evt.newValue;
            ItemListView.Rebuild();
        });

        //ItemName
        if (activeItem.ItemName != null)
            ItemDetailInfo.Q<TextField>("ItemName").value = activeItem.ItemName;
        ItemDetailInfo.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemName = evt.newValue;
            ItemListView.Rebuild();
        });

        //Type
        ItemDetailInfo.Q<EnumField>("ItemType").Init(new ItemType());
        ItemDetailInfo.Q<EnumField>("ItemType").value = activeItem.itemType;
        ItemDetailInfo.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
            ItemListView.Rebuild();
        });

        //Icon
        previewIcon = ItemDetailInfo.Q<VisualElement>("Icon");
        if (activeItem.itemIcon == null)
        {
            activeItem.itemIcon = defaultIcon;
            previewIcon.style.backgroundImage = defaultIcon.texture;
        }
        else
        {
            previewIcon.style.backgroundImage = activeItem.itemIcon.texture;
        }
        ItemDetailInfo.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        ItemDetailInfo.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemIcon = evt.newValue as Sprite;
            previewIcon.style.backgroundImage = (evt.newValue as Sprite).texture;
            ItemListView.Rebuild();
        });

        //OnWroldSprite
        if (activeItem.itemOnWorldSprite == null)
        {
            activeItem.itemIcon = defaultIcon;
        }
        
        ItemDetailInfo.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        ItemDetailInfo.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = evt.newValue as Sprite;
        });


        //Description
        ItemDetailInfo.Q<TextField>("Description").value = activeItem.itemDescription;
        ItemDetailInfo.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        //ItemUseRadius
        ItemDetailInfo.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        ItemDetailInfo.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        //CanPickedUp
        ItemDetailInfo.Q<Toggle>("CanPickedUp").value = activeItem.canPickedUp;
        ItemDetailInfo.Q<Toggle>("CanPickedUp").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedUp = evt.newValue;
        });

        //CanDropped
        ItemDetailInfo.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        ItemDetailInfo.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        //ItemUseRadius
        ItemDetailInfo.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        ItemDetailInfo.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        //itemPrice
        ItemDetailInfo.Q<IntegerField>("ItemPrice").value = activeItem.itemPrice;
        ItemDetailInfo.Q<IntegerField>("ItemPrice").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        //SellPercentage
        ItemDetailInfo.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        ItemDetailInfo.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }


}