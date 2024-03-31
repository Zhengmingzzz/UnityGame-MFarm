using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
    using MFarm.CropPlant;

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
        private Dictionary<string, bool> FirstLoadSceneDic = new();

        private Grid currentGrid;


        private Transform playerTransform => FindObjectOfType<Player>().transform;

        private List<ReapItem> reapItemList;


        protected override void Awake()
        {
            base.Awake();
            foreach (var t in mapData_SO_List)
            {
                GetmapDataToDic(t);
                if (!FirstLoadSceneDic.ContainsKey(t.SceneName))
                {
                    FirstLoadSceneDic.Add(t.SceneName, true);
                }
            }
        }
        private void Start()
        {
            


        }


        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.executeActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.UpdataGameDayEvent += OnUpdataGameDayEvent;
            EventHandler.RefleshMapDateEvent += OnRefleshMapDateEvent;

        }

        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.executeActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.UpdataGameDayEvent -= OnUpdataGameDayEvent;
            EventHandler.RefleshMapDateEvent -= OnRefleshMapDateEvent;

        }

        private void OnRefleshMapDateEvent()
        {
            RefreshMapDate();
        }

        private void GetmapDataToDic(MapData_SO mapData_SO)
        {
            foreach (TileProperty t in mapData_SO.TilePropertiesList)
            {
                string key = mapData_SO.SceneName + " " + t.gridX + "x" + t.gridY + "y";
                TileDetail tileDetail = getTileDetailByKey(key);

                if (tileDetail == null)
                {
                    tileDetail = new TileDetail
                    {
                        gridX = t.gridX,
                        gridY = t.gridY,
                    };
                }
                

                switch (t.gridType)
                {
                    case E_GridType.CanDig:
                        tileDetail.CanDig = true;
                        break;
                    case E_GridType.CanDrop:
                        tileDetail.CanDropItem = true;
                        break;
                    case E_GridType.CanPlaceFurniture:
                        tileDetail.CanPlaceFurniture = true;
                        break;
                    case E_GridType.NPC_Obstacle:
                        tileDetail.NPC_Obstacle = true;
                        break;
                }

                
                if (tileDetail != null)
                {
                    TileDetailDic[key] = tileDetail;
                }
                else
                {
                    TileDetailDic.Add(key, tileDetail);
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
        /// 执行动画播放后实现的功能
        /// </summary>
        /// <param name="clickedWorldPos">鼠标世界坐标信息</param>
        /// <param name="clickedItemDetail">选择物品信息</param>
        private void OnExecuteActionAfterAnimation(Vector3 clickedWorldPos, ItemDetails clickedItemDetail)
        {
            Vector3Int MousePositionInGrid = currentGrid.WorldToCell(clickedWorldPos);
            TileDetail currentTileDetail = getTileDetailByPos(MousePositionInGrid);
            Crop currentCrop = FindCropByMouseWorldPos(clickedWorldPos);
            if (clickedItemDetail.itemType == ItemType.ChopTool)
            {
                if (currentCrop != null)
                {
                    currentTileDetail = getTileDetailByPos(new Vector3Int(currentCrop.tileDetail.gridX, currentCrop.tileDetail.gridY, 0));
                }
            }

            if (currentTileDetail != null && clickedItemDetail != null)
            {
                switch (clickedItemDetail.itemType)
                {
                    //TODO:添加不同物品类型的实现
                    case ItemType.Commodity:
                        EventHandler.CallUpDropItemEvent(clickedItemDetail.ItemID, playerTransform.position, MousePositionInGrid) ;
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
                    case ItemType.Seed:
                        EventHandler.CallUpPlantEvent(clickedItemDetail.ItemID, currentTileDetail);
                        break;
                    case ItemType.ReapTool:
                        for (int i = 0; i < reapItemList.Count; i++)
                        {
                            reapItemList[i].SpawnCrop();
                            if (i >= Settings.ReapItemSpawCount)
                            {
                                break;
                            }
                        }
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        if (currentCrop != null)
                        {
                            currentCrop.ToolActionProcess(clickedItemDetail.ItemID, currentCrop.tileDetail);
                        }
                        break;
                    case ItemType.CollectionTool:
                        if (currentCrop != null)
                        {
                            currentCrop.ToolActionProcess(clickedItemDetail.ItemID, currentTileDetail);
                        }
                        break;
                }
                UpdataTileDetailToDic(currentTileDetail);
            }

            
        }

        /// <summary>
        /// 根据鼠标点击后的世界坐标并返回当前坐标下的种子(Crop)类型数据
        /// </summary>
        /// <param name="mouseClickedWorldPos"></param>
        /// <returns></returns>
        public Crop FindCropByMouseWorldPos(Vector3 mouseClickedWorldPos)
        {
            // 鼠标点击将所有的碰撞体添加到colliders中
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseClickedWorldPos);
            Crop currentCrop = null;

            foreach (Collider2D c in colliders)
            {
                Crop crop = c.gameObject.GetComponent<Crop>();
                if (crop != null)
                {
                    currentCrop = crop;
                    break;
                }
            }
            return currentCrop;
        }


        private void OnAfterLoadSceneEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            waterTileMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
            digTileMap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();

            if (FirstLoadSceneDic[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallUpSaveItemPrefabEvent();
                FirstLoadSceneDic[SceneManager.GetActiveScene().name] = false;
            }

            RefreshMapDate();
        }

        private void OnDigTile(TileDetail tileDetail)
        {
            digTileMap.SetTile(new Vector3Int(tileDetail.gridX, tileDetail.gridY, 0), digRuleTile);
        }
        private void OnWaterTile(TileDetail tileDetail)
        {
            waterTileMap.SetTile(new Vector3Int(tileDetail.gridX, tileDetail.gridY, 0), waterRuleTile);
        }


        public void UpdataTileDetailToDic(TileDetail tileDetail)
        {
            string key = SceneManager.GetActiveScene().name + " " + tileDetail.gridX + "x" + tileDetail.gridY + "y";

            if (TileDetailDic.ContainsKey(key))
            {
                TileDetailDic[key] = tileDetail;
            }
            else
            {
                TileDetailDic.Add(key, tileDetail);
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

                if (tile.Value.digSinceDay > -1 && tile.Value.seedSinceDay == -1)
                {
                    tile.Value.digSinceDay = -1;
                    tile.Value.CanDig = true;
                    tile.Value.seedID = -1;
                }
                if (tile.Value.seedSinceDay != -1)
                {
                    tile.Value.seedSinceDay++;
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
            foreach (var c in FindObjectsOfType<Crop>())
            {
                Destroy(c.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }

        private void DisplayMap(string SceneName)
        {
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

                    if (tileDetail.seedID != -1)
                    {
                        EventHandler.CallUpPlantEvent(tileDetail.seedID, tileDetail);
                    }
                }

            }

        }

        public bool CheckReapItemValidInRadium(ItemDetails toolDetails,Vector3 mouseWorldPos)
        {
            Collider2D[] colliderArray = new Collider2D[20];
            reapItemList = new List<ReapItem>();
            int count = Physics2D.OverlapCircleNonAlloc(mouseWorldPos, toolDetails.itemUseRadius,colliderArray);

            foreach (Collider2D c in colliderArray)
            {
                if(c!=null && c.GetComponent<ReapItem>())
                    reapItemList.Add(c.GetComponent<ReapItem>());
            }


            return reapItemList.Count > 0;
        }


        public bool getGridDimensions(string SceneName, out Vector2Int gridDimension, out Vector2Int gridOrigin)
        {
            gridDimension = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var m in mapData_SO_List)
            {
                
                if (m.SceneName == SceneName)
                {
                    gridDimension = new Vector2Int(m.gridWidth, m.gridHeight);
                    gridOrigin = new Vector2Int(m.originX, m.originY);
                    return true;
                }
            }
            return false;
        }



    }

    
}