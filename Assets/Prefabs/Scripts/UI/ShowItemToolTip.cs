using UnityEngine.EventSystems;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI => GetComponent<SlotUI>();
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.ItemAmount.text != "")
            {
                inventoryUI.itemToolTip.SetupToolTip(slotUI.itemDetail, slotUI.slotType);
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * inventoryUI.itemToolTip.UpNum;
                inventoryUI.itemToolTip.gameObject.SetActive(true);
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }


    }

}