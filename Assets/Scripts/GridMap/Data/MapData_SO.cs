using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO",menuName = "Map/MapData_SO")]
public class MapData_SO : ScriptableObject
{
    [SceneName]public string SceneName;
    [Header("地图信息")]
    public int gridHeight;
    public int gridWidth;
    [Header("最左下角网格坐标")]
    public int originX;
    public int originY;
    [Space(1)]

    public List<TileProperty> TilePropertiesList = new List<TileProperty>();
}
