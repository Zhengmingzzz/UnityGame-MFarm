using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemDetails
{
    public int ItemID;
    public string ItemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedUp;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0,1)]
    public float sellPercentage;

}

[System.Serializable]
public struct InventoryType
{
    public int ItemID;
    public int ItemAmount;
}

[System.Serializable]
public class AnimType
{
    public BodyTypeName bodyName;
    public NowState nowState;
    public AnimatorOverrideController animator;

}

public class sceneItems
{
    public SerializedVector3 itemPos;
    public int itemID;
}

[System.Serializable]
public class SerializedVector3
{
    float x, y, z;

    public SerializedVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// 网格的x,y坐标，类型，对应类型的bool值
/// </summary>
[System.Serializable]
public class TileProperty
{
    public int gridX, gridY;
    public E_GridType gridType;
    public bool gridTypeBoolValue;
}

/// <summary>
/// 记录了每个网格的信息，包括xy坐标，是否可以挖掘 丢物品... 种子的ID 种植了的时间以及农作物成长的时间
/// </summary>
[System.Serializable]
public class TileDetail
{
    public int gridX, gridY;
    public bool CanDig = false, CanDropItem = false, CanPlaceFurniture = false, NPC_Obstacle = false;

    public int digSinceDay = -1;
    public int wateredSinceDay = -1;
    public int seedID = -1;
    public int seedSinceDay = -1;
    public int harvestTimes = -1;
}

/// <summary>
/// 预制体信息，用于填充对象池
/// </summary>
[System.Serializable]
public struct S_ParticalEffect
{
    public E_PESType E_particalSystem;
    public GameObject ParticalEffectPrefab;
}

[System.Serializable]
public class NPC_Position
{
    public Transform NPCTransform;
    public string SceneName;
    public Vector3Int StartPosition;
}

/// <summary>
/// 含有多个ScenePath类
/// 用于记录NPC在多个场景移动时需要到达的坐标
/// </summary>
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string toSceneName;
    public List<ScenePath> SecneRouteList;
}

/// <summary>
/// 记录了NPC在某一场景中的移动坐标路径
/// 若其中一个坐标为9999则表示到达该场景后按schedule中的坐标移动
/// </summary>
[System.Serializable]
public class ScenePath
{
    // 用于在RouteData_SO文件中了解当前场景
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}