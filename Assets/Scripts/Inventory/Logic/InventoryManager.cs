using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//与背包 数据有关的都引用到一个命名空间
namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("所有物品数据")]
        public ItemDetailList_SO itemDetailList;

        [Header("背包数据")]
        public InventoryBag_SO playerBag;




        private void Start()
        {
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestCropOnPlayer += OnHarvestCropOnPlayer;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestCropOnPlayer -= OnHarvestCropOnPlayer;

        }

        private void OnHarvestCropOnPlayer(int seedID)
        {
            int Exist_Index = isExistItem(seedID);
            int NULL_Index = bagIsFull();



            InventoryType newBagPos;
            if (Exist_Index != -1)
            {
                newBagPos.ItemID = seedID;
                newBagPos.ItemAmount = playerBag.itemList[Exist_Index].ItemAmount + 1;
                playerBag.itemList[Exist_Index] = newBagPos;

            }
            else if (NULL_Index == -1)
            {
                return;
            }
            else if (NULL_Index != -1)
            {
                newBagPos.ItemID = seedID;
                newBagPos.ItemAmount = 1;
                playerBag.itemList[NULL_Index] = newBagPos;
            }

            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }

        public ItemDetails GetItemDetailsByID(int ItemID)
        {
            foreach (ItemDetails t in itemDetailList.ItemDetailsList)
            {
                if (t.ItemID == ItemID)
                {
                    return t;
                }
            }
            return null;
        }

        public int GetPlayerBagItemIndexByID(int ItemID)
        {

            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].ItemID == ItemID)
                {
                    return i;
                }
            }
            return -1;
        }

        public int bagIsFull()
        {
            int count = 0;
            foreach (InventoryType i in playerBag.itemList)
            {
                if (i.ItemAmount == 0)
                {
                    return count;
                }
                count++;
            }
            return -1;
        }
        int isExistItem(int itemID)
        {
            int count = 0;
            foreach (InventoryType i in playerBag.itemList)
            {
                if (itemID == i.ItemID)
                {
                    return count;
                }
                count++;
            }
            return -1;
        }

        

        public void PickedUpItem(RenderItem item , bool toDestroy)
        {
            

            int Exist_Index = isExistItem(item.ItemID);
            int NULL_Index = bagIsFull();



            InventoryType newBagPos;

            if (Exist_Index != -1)
            {
                newBagPos.ItemID = item.ItemID;
                newBagPos.ItemAmount = playerBag.itemList[Exist_Index].ItemAmount + 1;
                playerBag.itemList[Exist_Index] = newBagPos;

            }
            else if (NULL_Index == -1)
            {
                return;
            }
            else if(NULL_Index != -1)
            {
                newBagPos.ItemID = item.ItemID;
                newBagPos.ItemAmount = 1;
                playerBag.itemList[NULL_Index] = newBagPos;
            }
            

                //添加数据到背包
            
            if (toDestroy == true)
            {
                Destroy(item.gameObject);
            }


            //更新UI
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        public void SortBag()
        {
            InventoryType t;

            for (int i = playerBag.itemList.Count - 1; i >= 0; i--)
            {
                
                for (int j = 0; j < i - 1; j++)
                {
                    if (playerBag.itemList[j + 1].ItemID > playerBag.itemList[j].ItemID)
                    {
                        t = playerBag.itemList[j + 1];
                        playerBag.itemList[j + 1] = playerBag.itemList[j];
                        playerBag.itemList[j] = t;
                    }
                }
            }
        }

        public void SwapItem(int from,int to)
        {
            InventoryType current = playerBag.itemList[from];
            InventoryType target = playerBag.itemList[to];

            if (playerBag.itemList[to].ItemAmount != 0)
            {
                playerBag.itemList[from] = target;
                playerBag.itemList[to] = current;
            }
            else
            {
                playerBag.itemList[from] = new InventoryType();
                playerBag.itemList[to] = current;
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        public void RemoveItemAmount(int itemID, int RemoveAmount)
        {
            int itemIndex = GetPlayerBagItemIndexByID(itemID);
            if (itemIndex != -1)
            {
                InventoryType currentItemInfo = playerBag.itemList[itemIndex];

                if (currentItemInfo.ItemAmount != 0)
                {
                    if (currentItemInfo.ItemAmount > RemoveAmount)
                    {
                        playerBag.itemList[itemIndex] = new InventoryType { ItemAmount = currentItemInfo.ItemAmount - RemoveAmount, ItemID = itemID };
                    }
                    else if (currentItemInfo.ItemAmount == RemoveAmount)
                    {
                        playerBag.itemList[itemIndex] = new InventoryType();
                    
                    }
                    EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

                }
            }

            
        }

        private void OnDropItemEvent(int itemID, Vector3 GridPos)
        {
            RemoveItemAmount(itemID, 1);
        }

    }
}
    
    



