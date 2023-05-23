using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryType>> UpdataInventoryUI;
    public static void CallUpdataInventoryUI(InventoryLocation ItemLocation, List<InventoryType> itemList)
    {
        UpdataInventoryUI?.Invoke(ItemLocation, itemList);
    }



    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallUpInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }

    public static event Action<int, Vector3> DropItemEvent;
    public static void CallUpDropItemEvent(int itemID, Vector3 targetPos)
    {
        DropItemEvent.Invoke(itemID, targetPos);
    }


    public static event Action<ItemDetails, bool> ItemSelectEvent;
    public static void CallUpItemSelectEvent(ItemDetails itemdetails, bool isSelect)
    {
        ItemSelectEvent?.Invoke(itemdetails, isSelect);
    }




    public static event Action<int, int> UpdataTimeUI;
    public static void CallUpUpdataTimeUI(int minute, int hour)
    {
        UpdataTimeUI?.Invoke(minute, hour);
    }

    public static Action<int, Season> UpdataGameDayEvent;
    public static void CallUpUpdataGameDayEvent(int GameDay, Season season)
    {
        UpdataGameDayEvent?.Invoke(GameDay, season);
    }


    public static event Action<int, int, int, Season> UpdataDate;
    public static void CallUpUpdataDate(int year, int mouth, int day, Season season)
    {
        EventHandler.UpdataDate(year, mouth, day, season);
    }



    public static event Action<bool> timeAccelerate;
    public static void CallUptimeAccelerate(bool isAccelerate)
    {
        timeAccelerate?.Invoke(isAccelerate);
    }


    public static event Action<string, Vector3> TransitionSceneEvent;
    public static void CallUpTransitionSceneEvent(string newSceneName,Vector3 newScenePos)
    {
        TransitionSceneEvent?.Invoke(newSceneName, newScenePos) ;
    }


    public static event Action BeforeUnLoadSceneEvent;
    public static void CallUpBeforeUnLoadSceneEvent()
    {
        BeforeUnLoadSceneEvent?.Invoke();
    }


    public static event Action AfterLoadSceneEvent;
    public static void CallUpAfterLoadSceneEvent()
    {
        AfterLoadSceneEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToNewSceneEvent;
    public static void CallUpMoveToNewSceneEvent(Vector3 newScenePos)
    {
        MoveToNewSceneEvent?.Invoke(newScenePos);
    }

    public static event Action<Vector3, ItemDetails> mouseClickedEvent;
    public static void CallUpMouseClickedEvent(Vector3 mouseClickedWorldPosition, ItemDetails itemDetails)
    {
        mouseClickedEvent?.Invoke(mouseClickedWorldPosition, itemDetails);
    }

    public static event Action<Vector3, ItemDetails> executeActionAfterAnimation;
    public static void CallUpExecuteActionAfterAnimation(Vector3 mouseClickedWorldPosition, ItemDetails itemDetails)
    {
        executeActionAfterAnimation?.Invoke(mouseClickedWorldPosition, itemDetails);
    }

    public static event Action<int, TileDetail> PlantEvent;
    public static void CallUpPlantEvent(int seedID, TileDetail tileDetail)
    {
        PlantEvent?.Invoke(seedID, tileDetail);
    }





}
