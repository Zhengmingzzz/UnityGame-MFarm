using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("ToolTipObject")]
        public ItemToolTip itemToolTip;
        [Header("拖拽图片")]
        public Image DropImage;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        bool bagIsOpen;

        [SerializeField]public SlotUI[] playerBag;

        private void Start()
        {

            for (int i = 0; i < playerBag.Length; i++)
            {
                playerBag[i].SlotIndex = i;
            }

            bagIsOpen = bagUI.activeInHierarchy;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                OpenBagUI();
            }
        }


        private void OnEnable()
        {
            EventHandler.UpdataInventoryUI += OnUpdataInvectoryUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdataInventoryUI -= OnUpdataInvectoryUI;
        }

        private void OnUpdataInvectoryUI(InventoryLocation Location , List<InventoryType> inventoryList)
        {
            //TODO:添加其他物品栏位置的信息更新方法
            switch (Location)
            {

                case InventoryLocation.Player:
                    for (int i = 0; i < inventoryList.Count; i++)
                    {
                        if (inventoryList[i].ItemAmount <= 0)
                        {
                            playerBag[i].UpdataEmptySlot();
                        }
                        else
                        {
                            playerBag[i].UpdataSlot(InventoryManager.Instance.GetItemDetailsByID(inventoryList[i].ItemID), inventoryList[i].ItemAmount);
                        }
                    }
                    break;
            }
        }

        public void OpenBagUI()
        {
            bagIsOpen = !bagIsOpen;
            if (bagIsOpen)
            {
                EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);
            }
            bagUI.SetActive(bagIsOpen);

        }

        public void UpdataSlotHightLight(int Index)
        {
            foreach (SlotUI s in playerBag)
            {
                if (s.isSelect && Index == s.SlotIndex)
                {
                    s.ItemHightLight.gameObject.SetActive(true);
                }
                else
                {
                    s.ItemHightLight.gameObject.SetActive(false);
                    s.isSelect = false;
                }
            }
        }

    }
}