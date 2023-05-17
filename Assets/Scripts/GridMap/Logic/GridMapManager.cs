using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        public List<MapData_SO> mapData_SO_List;
        private Dictionary<string, GridDetail> GridDetailDic = new Dictionary<string, GridDetail>();

        private void Start()
        {
            foreach (var t in mapData_SO_List)
            {
                GetmapDataToDic(t);
            }
        }


        private void OnEnable()
        {
            EventHandler.executeActionAfterAnimation += OnExecuteActionAfterAnimation;
        }

        private void OnDisable()
        {
            EventHandler.executeActionAfterAnimation -= OnExecuteActionAfterAnimation;

        }


        private void GetmapDataToDic(MapData_SO mapData_SO)
        {
            foreach (TileProperty t in mapData_SO.TilePropertiesList)
            {
                GridDetail gridDetail = new GridDetail
                {
                    gridX = t.gridX,
                    gridY = t.gridY,
                };

                switch (t.gridType)
                {
                    case E_GridType.CanDig:
                        gridDetail.CanDig = true;
                        break;
                    case E_GridType.CanDrop:
                        gridDetail.CanDropItem = true;
                        break;
                    case E_GridType.CanPlaceFurniture:
                        gridDetail.CanPlaceFurniture = true;
                        break;
                    case E_GridType.NPC_Obstacle:
                        gridDetail.NPC_Obstacle = true;
                        break;
                }

                string key = mapData_SO.SceneName + " " + t.gridX + "x" + t.gridY + "y";
                GridDetail gridDe = getGridDetailByKey(key);
                if (gridDe != null)
                {
                    GridDetailDic[key] = gridDetail;
                }
                else
                {
                    GridDetailDic.Add(key, gridDetail);
                }
            }
        }

        private GridDetail getGridDetailByKey(string key)
        {
            if (GridDetailDic.ContainsKey(key))
            {
                return GridDetailDic[key];
            }


            return null;
        }


        public GridDetail getGridDetailByPos(Vector3Int pos)
        {
            return getGridDetailByKey(SceneManager.GetActiveScene().name + " " + pos.x + "x" + pos.y + "y");
        }


        private void OnExecuteActionAfterAnimation(Vector3 clickedWorldPos, ItemDetails clickedItemDetail)
        {
            switch (clickedItemDetail.itemType)
            {
                case ItemType.Commodity:
                    EventHandler.CallUpInstantiateItemInScene(clickedItemDetail.ItemID, clickedWorldPos);

                    InventoryType i = Inventory.InventoryManager.Instance.playerBag.itemList[clickedItemDetail.ItemID];
                    if (i.ItemAmount <= 1)
                    {
                        i.ItemAmount--;
                        i.ItemID = 0;
                    }
                    else
                    {
                        i.ItemAmount--;
                    }
                    Inventory.InventoryManager.Instance.playerBag.itemList[clickedItemDetail.ItemID] = i;


                    EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, Inventory.InventoryManager.Instance.playerBag.itemList);
                    break;
            }
        }

    }


}