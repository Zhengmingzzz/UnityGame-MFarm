using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

public class ItemGenerator : MonoBehaviour
{
    public int seedID;
    public int glowthDays;
    private Grid currentGrid;

    private void Awake()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    private void OnEnable()
    {
        EventHandler.SaveItemPrefabEvent += SaveDateToGridMap;
    }
    private void OnDestroy()
    {
        EventHandler.SaveItemPrefabEvent -= SaveDateToGridMap;

    }
    private void SaveDateToGridMap()
    {
        if (this.seedID != 0)
        {
            Vector3Int itemPos = currentGrid.WorldToCell(transform.position);
            TileDetail tile = GridMapManager.Instance.getTileDetailByPos(itemPos);

            if (tile == null)
            {
                tile = new TileDetail();
                tile.gridX = itemPos.x;
                tile.gridY = itemPos.y;
            }
            tile.CanDig = false;
            tile.seedSinceDay = glowthDays;
            tile.seedID = this.seedID;
            tile.digSinceDay = 0;



            GridMapManager.Instance.UpdataTileDetailToDic(tile);

        }

    }
}
