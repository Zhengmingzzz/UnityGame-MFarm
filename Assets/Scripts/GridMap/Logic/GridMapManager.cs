using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        public List<MapData_SO> mapData_SO_List;
        private Dictionary<string, TileDetail> TileDetailDic = new Dictionary<string, TileDetail>();

        private Grid currentGrid;



        private void Start()
        {
            foreach (var t in mapData_SO_List)
            {
                GetmapDataToDic(t);
            }
        }


        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.executeActionAfterAnimation += OnExecuteActionAfterAnimation;
        }

        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.executeActionAfterAnimation -= OnExecuteActionAfterAnimation;

        }


        private void GetmapDataToDic(MapData_SO mapData_SO)
        {
            foreach (TileProperty t in mapData_SO.TilePropertiesList)
            {
                TileDetail TileDetail = new TileDetail
                {
                    gridX = t.gridX,
                    gridY = t.gridY,
                };

                switch (t.gridType)
                {
                    case E_GridType.CanDig:
                        TileDetail.CanDig = true;
                        break;
                    case E_GridType.CanDrop:
                        TileDetail.CanDropItem = true;
                        break;
                    case E_GridType.CanPlaceFurniture:
                        TileDetail.CanPlaceFurniture = true;
                        break;
                    case E_GridType.NPC_Obstacle:
                        TileDetail.NPC_Obstacle = true;
                        break;
                }

                string key = mapData_SO.SceneName + " " + t.gridX + "x" + t.gridY + "y";
                TileDetail gridDe = getTileDetailByKey(key);
                if (gridDe != null)
                {
                    TileDetailDic[key] = TileDetail;
                }
                else
                {
                    TileDetailDic.Add(key, TileDetail);
                }
            }
        }

        private TileDetail getTileDetailByKey(string key)
        {
            if (TileDetailDic.ContainsKey(key))
            {
                return TileDetailDic[key];
            }


            return null;
        }


        public TileDetail getTileDetailByPos(Vector3Int GridPos)
        {
            return getTileDetailByKey(SceneManager.GetActiveScene().name + " " + GridPos.x + "x" + GridPos.y + "y");
        }

        /// <summary>
        /// 执行实际功能
        /// </summary>
        /// <param name="clickedWorldPos">鼠标世界坐标信息</param>
        /// <param name="clickedItemDetail">选择物品信息</param>
        private void OnExecuteActionAfterAnimation(Vector3 clickedWorldPos, ItemDetails clickedItemDetail)
        {
            Vector3Int MousePositionInGrid = currentGrid.WorldToCell(clickedWorldPos);
            TileDetail currentTileDetail = getTileDetailByPos(MousePositionInGrid);

            if (currentTileDetail != null)
            {
                switch (clickedItemDetail.itemType)
                {
                    case ItemType.Commodity:
                        EventHandler.CallUpDropItemEvent(clickedItemDetail.ItemID, MousePositionInGrid);
                        break;
                }



            }

            
        }


        private void OnAfterLoadSceneEvent()
        {
            currentGrid = GameObject.FindObjectOfType<Grid>();
        }

    }


}