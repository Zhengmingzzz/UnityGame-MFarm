using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] Image ItemImage;
        [SerializeField] Button ItemButton;
        public Image ItemHightLight;
        [SerializeField] public TextMeshProUGUI ItemAmount;

        [Header("格子类型")]
        [SerializeField] public SlotType slotType;
        public bool isSelect;

        //物品信息
        public int SlotIndex;
        public ItemDetails itemDetail;



        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();


        private void Start()
        {
            isSelect = false;
        }
        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        }
        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        }

        private void OnAfterLoadSceneEvent()
        {
            ItemHightLight.gameObject.SetActive(false);

        }

        public void UpdataEmptySlot()
        {
            if (isSelect)
            {
                isSelect = false;
                itemDetail = null;
                EventHandler.CallUpItemSelectEvent(itemDetail, isSelect);

            }
            ItemImage.enabled = false;
            ItemButton.interactable = false;
            ItemHightLight.gameObject.SetActive(false);
            ItemAmount.text = "";
        }

        public void UpdataSlot(ItemDetails item , int itemAmount)
        {
            
            ItemImage.sprite = item.itemOnWorldSprite;
            ItemImage.enabled = true;
            ItemButton.interactable = true;
            ItemAmount.text = Convert.ToString(itemAmount);

            itemDetail = item;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ItemAmount.text == "")
            {
                return;
            }
            isSelect = !isSelect;
            inventoryUI.UpdataSlotHightLight(SlotIndex);

            if (slotType == SlotType.Bag)
            {
                EventHandler.CallUpItemSelectEvent(itemDetail, isSelect);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (ItemAmount.text != "")
            {
                inventoryUI.DropImage.enabled = true;
                inventoryUI.DropImage.sprite = ItemImage.sprite;

                isSelect = true;
                inventoryUI.UpdataSlotHightLight(SlotIndex);

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.DropImage.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.DropImage.enabled = false;
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                var target = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                if (target != null)
                {
                    int targetIndex = target.SlotIndex;


                    if (slotType == SlotType.Bag && target.slotType == SlotType.Bag)
                    {
                        InventoryManager.Instance.SwapItem(SlotIndex, targetIndex);
                    }
                }

            }
            else
            {
                if (itemDetail.canDropped)
                {
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    EventHandler.CallUpInstantiateItemInScene(itemDetail.ItemID, pos);

                    InventoryType i = InventoryManager.Instance.playerBag.itemList[SlotIndex];
                    if (i.ItemAmount <= 1)
                    {
                        i.ItemAmount--;
                        i.ItemID = 0;
                    }
                    else
                    {
                        i.ItemAmount--;
                    }
                    InventoryManager.Instance.playerBag.itemList[SlotIndex] = i;


                    EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);
                }

            }


            //取消所有高亮
            inventoryUI.UpdataSlotHightLight(-1);
        }
    }

}