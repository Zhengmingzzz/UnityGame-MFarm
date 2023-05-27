using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [Header("种子信息")]
    public int seedID;
    public Season[] season;
    public int[] growthDays;
    public int TotalGlowthDays { 
        get 
        {
            int amount = 0;
            foreach (int day in growthDays)
            {
                amount += day;
            }
            return amount;
        } }
    public GameObject[] seedPrefabs;
    public Sprite[] seedSprite;



    [Space]
    [Header("收割工具")]
    public int[] harvestToolID;
    public int[] harvestActionCount;
    public int TransferNewItemID;



    [Space]
    [Header("收获果实信息")]
    public int[] productedItemID;
    public int[] MinAmount;
    public int[] MaxAmount;
    public int spawnRadius;

    [Space]
    [Header("再次生长时间")]
    public int dayToReglow;
    public int ReglowTimes;

    [Header("其他选项")]
    public bool GenarateAtPlayerHead;
    public bool haveAnimation;
    public bool particalEffect;
    public E_PESType[] ParticalEffectSystem;


    public bool CheckToolValid(int toolID)
    {
        foreach (int i in harvestToolID)
        {
            if (i == toolID)
            {
                return true;
            }
        }
        return false;
    }

}
