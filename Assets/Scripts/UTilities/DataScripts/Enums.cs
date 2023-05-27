using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum ItemType
{
    Seed,Commodity,Furniture,
    //锄头  砍树工具  砸石头工具 割草工具 浇水     收割工具
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectionTool,
    //可被割的物品（杂草）
    ReapableScenery

}

public enum SlotType
{
    Bag,Shop,Box
}



public enum InventoryLocation
{
    Player,Box,Shop
}

public enum BodyTypeName
{
    Arm,Body,Hair,Tool
}

public enum NowState
{
    Carry,None,Hoe,Water,Harvest
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum E_GridType
{
    CanDig,CanDrop, CanPlaceFurniture,NPC_Obstacle,
}

public enum E_PESType
{
    None,LeaveFalling01,LeaveFalling02,Rock,
}