using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("RuleTile")]
        public RuleTile waterRuleTile;
        public RuleTile digRuleTile;
        private Tilemap waterTileMap;
        private Tilemap digTileMap;

        [Header("场景信息")]
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
            EventHandler.UpdataGameDayEvent += OnUpdataGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.executeActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.UpdataGameDayEvent -= OnUpdataGameDayEvent;

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

            if (currentTileDetail != null && clickedItemDetail != null)
            {
                switch (clickedItemDetail.itemType)
                {
                    //TODO:添加其他类型的实现
                    case ItemType.Commodity:
                        EventHandler.CallUpDropItemEvent(clickedItemDetail.ItemID, MousePositionInGrid);
                        break;
                    case ItemType.HoeTool:
                        OnDigTile(currentTileDetail);
                        currentTileDetail.digSinceDay = 0;
                        currentTileDetail.CanDig = false;
                        currentTileDetail.CanDropItem = false;
                        break;
                    case ItemType.WaterTool:
                        OnWaterTile(currentTileDetail);
                        currentTileDetail.wateredSinceDay = 0;
                        break;
                }
                UpdataTileDetailToDic(currentTileDetail);
            }

            
        }


        private void OnAfterLoadSceneEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            waterTileMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
            digTileMap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();

            DisplayMap(SceneManager.GetActiveScene().name);
        }

        private void OnDigTile(TileDetail tileDetail)
        {
            digTileMap.SetTile(new Vector3Int(tileDetail.gridX, tileDetail.gridY, 0), digRuleTile);
        }
        private void OnWaterTile(TileDetail tileDetail)
        {
            waterTileMap.SetTile(new Vector3Int(tileDetail.gridX, tileDetail.gridY, 0), waterRuleTile);
        }


        private void UpdataTileDetailToDic(TileDetail tileDetail)
        {
            string key = SceneManager.GetActiveScene().name + " " + tileDetail.gridX + "x" + tileDetail.gridY + "y";

            if (TileDetailDic.ContainsKey(key))
            {
                TileDetailDic[key] = tileDetail;
            }
        }


        private void DisplayMap(string SceneName)
        {
            //TODO:后续需要保存种子信息
            foreach (var tile in TileDetailDic)
            {
                string key = tile.Key;
                TileDetail tileDetail = tile.Value;

                if (key.Contains(SceneName))
                {
                    if (tileDetail.digSinceDay != -1)
                    {
                        OnDigTile(tileDetail);
                    }
                    if (tileDetail.wateredSinceDay != -1)
                    {
                        OnWaterTile(tileDetail);
                    }
                }

            }

        }


        private void OnUpdataGameDayEvent(int gameDay, Season gameSeason)
        {
            foreach (var tile in TileDetailDic)
            {
                if (tile.Value.digSinceDay > -1)
                {
                    tile.Value.digSinceDay++;
                }
                if (tile.Value.wateredSinceDay > -1)
                {
                    tile.Value.wateredSinceDay = -1;
                }

                //TODO:测试
                if (tile.Value.digSinceDay > -1 && tile.Value.seedSinceDay == -1)
                {
                    tile.Value.digSinceDay = -1;
                    tile.Value.CanDig = true;
                }

            }

            RefreshMapDate();
        }

        private void RefreshMapDate()
        {
            if (digTileMap != null)
            {
                digTileMap.ClearAllTiles();
            }
            if (waterTileMap != null)
            {
                waterTileMap.ClearAllTiles();
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }









    }

    
}