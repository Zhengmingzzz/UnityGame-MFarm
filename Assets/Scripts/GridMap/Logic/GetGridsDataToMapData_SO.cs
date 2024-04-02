using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;


namespace MFarm.Map
{

    [ExecuteInEditMode]
    public class GetGridsDataToMapData_SO : MonoBehaviour
    {
        public MapData_SO mapData_SO;
        public E_GridType GridType;
        private Tilemap tilemap;


        //在开启和关闭是都要刷新地图数据到mapData_SO的list中
        private void OnEnable()
        {
            if (!Application.IsPlaying(this))
            {
                tilemap = GetComponent<Tilemap>();
                if (mapData_SO != null)
                    mapData_SO.TilePropertiesList.Clear();
            }
        }

        private void OnDisable()
        {
            if (!Application.IsPlaying(this))
            {
                tilemap = GetComponent<Tilemap>();

                UpdataMapDataToMapData_SO();

#if UNITY_EDITOR
                if (mapData_SO != null)
                {
                    EditorUtility.SetDirty(mapData_SO);
                }
#endif
            }

        }


        private void UpdataMapDataToMapData_SO()
        {
            tilemap.CompressBounds();

            if (tilemap != null)
            {

                Vector3Int minPos = tilemap.cellBounds.min;
                Vector3Int maxPos = tilemap.cellBounds.max;


                for (int x = minPos.x; x < maxPos.x; x++)
                {
                    for (int y = minPos.y; y < maxPos.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            TileProperty tileProperty = new TileProperty
                            {
                                gridX = x,
                                gridY = y,
                                gridType = GridType,
                                gridTypeBoolValue = true,
                            };



                            mapData_SO.TilePropertiesList.Add(tileProperty);
                        }
                    }
                }
            }






        }

    }
}