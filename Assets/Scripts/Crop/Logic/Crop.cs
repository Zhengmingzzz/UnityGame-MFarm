using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MFarm.CropPlant
{


    public class Crop : MonoBehaviour
    {
        public CropDetails cropDetails;

        private int harvestActionCount;

        public TileDetail tileDetail;

        private Animator animator;
        public bool canHarvest => cropDetails.TotalGlowthDays <= tileDetail.seedSinceDay;

        private Transform playerTransform => FindObjectOfType<Player>().transform;

        bool isusingAnimation = false;


        public void ToolActionProcess(int toolID, TileDetail tileDetail)
        {
            this.tileDetail = tileDetail;
            int requirActionCount = GetRequirCount(toolID);
            if (requirActionCount == -1)
            {
                return;

            }

            if (harvestActionCount < requirActionCount)
            {

                harvestActionCount++;

                //TODO:播放动画
                if (cropDetails.haveAnimation)
                {
                    animator = this.GetComponentInChildren<Animator>();


                    if (animator != null && playerTransform != null)
                    {
                        if (playerTransform.position.x > tileDetail.gridX)
                            animator.SetTrigger("Cut_Left");
                        else
                            animator.SetTrigger("Cut_Right");
                    }

                }
                //TODO:产生粒子效果
                if (cropDetails.particalEffect)
                {
                    for (int i = 0; i < cropDetails.ParticalEffectSystem.Length; i++)
                    {

                        EventHandler.CallUpPEInstantiateEvent(cropDetails.ParticalEffectSystem[i], new Vector3(tileDetail.gridX, tileDetail.gridY, 0));

                    }
                }
            }
            else
            {
                if (animator != null && playerTransform != null && !isusingAnimation)
                {
                    if (playerTransform.position.x > this.transform.position.x)
                        animator.SetTrigger("CutDown_Left");
                    else
                        animator.SetTrigger("CutDown_Right");
                    StartCoroutine(HarvestAfterAniamtion());

                }
                else
                {
                    SpawnCrop();
                }
            }
        }

        IEnumerator HarvestAfterAniamtion()
        {
            isusingAnimation = true;
            if (cropDetails.haveAnimation)
            {
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("END"))
                {
                    yield return null;
                }

                SpawnCrop();
            }
            isusingAnimation = false;

        }


        private int GetRequirCount(int toolID)
        {
            for (int i = 0; i < cropDetails.harvestToolID.Length; i++)
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
                        int minX = tileDetail.gridX - cropDetails.spawnRadius;
                        int maxX = tileDetail.gridX + cropDetails.spawnRadius;

                        int minY = tileDetail.gridY - cropDetails.spawnRadius;
                        int maxY = tileDetail.gridY + cropDetails.spawnRadius;

                        Vector3 RandomTargetPos = Vector3.zero;

                        if (playerTransform.position.x > this.transform.position.x)
                        {
                            RandomTargetPos = new Vector3(Random.Range(minX, tileDetail.gridX), Random.Range(minY, maxY + 1));

                        }
                        else
                        {
                            RandomTargetPos = new Vector3(Random.Range(tileDetail.gridX, maxX + 1), Random.Range(minY, maxY + 1));

                        }



                        EventHandler.CallUpDropItemEvent(cropDetails.productedItemID[i], new Vector3(tileDetail.gridX, tileDetail.gridY, 0), RandomTargetPos);
                    }
                }

                if (tileDetail != null)
                {
                    tileDetail.harvestTimes++;

                    if (cropDetails.ReglowTimes > tileDetail.harvestTimes + 1)
                    {
                        tileDetail.seedSinceDay -= cropDetails.dayToReglow;
                    }
                    else if (cropDetails.TransferNewItemID != -1)
                    {
                        tileDetail.seedID = cropDetails.TransferNewItemID;
                    }
                    else
                    {
                        tileDetail.harvestTimes = -1;
                        tileDetail.seedID = -1;
                        tileDetail.seedSinceDay = -1;
                    }


                    if (cropDetails.TransferNewItemID > 0)
                    {
                        CropManager.Instance.DisplayPlant(CropManager.Instance.GetCropDetailsByID(cropDetails.TransferNewItemID), tileDetail);
                    }


                    EventHandler.CallUpRefleshMapDateEvent();

                }


            }

        }


    }
}