using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO",menuName = "Map/MapData_SO")]
public class MapData_SO : ScriptableObject
{
    [SceneName]public string SceneName;
    public List<TileProperty> TilePropertiesList = new List<TileProperty>();
}
