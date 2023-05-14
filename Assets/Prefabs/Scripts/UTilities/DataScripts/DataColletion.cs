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
    public AnimatorOverrideController animation;

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



