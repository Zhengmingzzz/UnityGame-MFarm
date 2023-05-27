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

[System.Serializable]
public class TileProperty
{
    public int gridX, gridY;
    public E_GridType gridType;
    public bool gridTypeBoolValue;
}

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

[System.Serializable]
public struct S_ParticalEffect
{
    public E_PESType E_particalSystem;
    public GameObject ParticalEffectPrefab;
}