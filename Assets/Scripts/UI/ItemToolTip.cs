using UnityEngine.UI;
using TMPro;
using UnityEngine;


namespace MFarm.Inventory
{
    public class ItemToolTip : MonoBehaviour
    {
        public int UpNum;
        [SerializeField] TextMeshProUGUI ItemName;
        [SerializeField] TextMeshProUGUI M_ItemType;
        [SerializeField] TextMeshProUGUI ItemDescription;
        [SerializeField] Text ItemPrice;


        public void SetupToolTip(ItemDetails itemDetail, SlotType slotType)
        {
            ItemName.text = itemDetail.ItemName;
            M_ItemType.text = GetItemType(itemDetail.itemType);
            ItemDescription.text = itemDetail.itemDescription;

            if (itemDetail.itemType == global::ItemType.Seed || itemDetail.itemType == global::ItemType.Commodity || itemDetail.itemType == global::ItemType.Furniture)
            {
                float price = itemDetail.itemPrice;
                if (slotType == SlotType.Shop)
                {
                    price *= itemDetail.sellPercentage;
                }

                ItemPrice.text = price.ToString();
            }
            else
            {
                ItemPrice.text = "无法出售:)";
            }
        }



        private string GetItemType(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Seed => "种子",
                ItemType.Commodity => "商品",
                ItemType.Furniture => "家具",
                ItemType.BreakTool => "工具",
                ItemType.ChopTool => "工具",
                ItemType.CollectionTool => "工具",
                ItemType.HoeTool => "工具",
                ItemType.ReapTool => "工具",
                ItemType.WaterTool => "工具",
                _=>"无"
            };
        }
    }
}