using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;

    private int harvestActionCount;

    private TileDetail tileDetail;


    public void ToolActionProcess(int toolID,TileDetail tileDetail)
    {
        this.tileDetail = tileDetail;
        int requirActionCount = GetRequirCount(toolID);
        if (requirActionCount == -1) return;


        if (harvestActionCount < requirActionCount)
        {
            harvestActionCount++;

            //TODO:播放动画
            //TODO:产生粒子效果
        }
        if (harvestActionCount >= requirActionCount)
        {
            SpawnCrop();


        }
    }

    private int GetRequirCount(int toolID)
    {
        for (int i = 0; i<cropDetails.harvestToolID.Length;i++)
        {
            if (cropDetails.harvestToolID[i] == toolID)
            {
                return cropDetails.harvestActionCount[i];
            }
        }
        return -1;
    }


    private void SpawnCrop()
    {
        for (int i = 0; i < cropDetails.productedItemID.Length; i++)
        {
            int amount;
            if (cropDetails.MinAmount[i] == cropDetails.MaxAmount[i])
            {
                amount = cropDetails.MinAmount[i];
            }
            else
            {
                amount = Random.Range(cropDetails.MinAmount[i], cropDetails.MaxAmount[i] + 1);
            }


            for (int j = 0; j < amount; j++)
            {
                //是在玩家头顶生成    
                //1 生成物品在背包中
                //2 生成物品在角色头顶
                if (cropDetails.GenarateAtPlayerHead)
                {
                    EventHandler.CallUpHarvestCropOnPlayer(cropDetails.productedItemID[i]);
                }
                //否则
                //生成物品在地图中
                else
                {

                }
            }

            if (tileDetail != null)
            {
                tileDetail.harvestTimes++;

                if (cropDetails.ReglowTimes > tileDetail.harvestTimes + 1)
                {
                    tileDetail.seedSinceDay -= cropDetails.dayToReglow;
                }
                else
                {
                    tileDetail.harvestTimes = -1;
                    tileDetail.seedID = -1;
                    tileDetail.seedSinceDay = -1;
                }

                EventHandler.CallUpRefleshMapDateEvent();

            }


        }

    }



}
