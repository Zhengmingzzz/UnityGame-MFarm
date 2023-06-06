using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{


    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;

        public void InitReapItem(int ID)
        {
            cropDetails = MFarm.CropPlant.CropManager.Instance.GetCropDetailsByID(ID);
        }

        public void SpawnCrop()
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
                    int minX = (int)this.transform.position.x - cropDetails.spawnRadius;
                    int maxX = (int)this.transform.position.x + cropDetails.spawnRadius;

                    int minY = (int)this.transform.position.y - cropDetails.spawnRadius;
                    int maxY = (int)this.transform.position.y + cropDetails.spawnRadius;

                    Vector3 RandomTargetPos = Vector3.zero;

                    if ((int)this.transform.position.x > this.transform.position.x)
                    {
                        RandomTargetPos = new Vector3(Random.Range(minX, (int)this.transform.position.x), Random.Range(minY, maxY + 1));

                    }
                    else
                    {
                        RandomTargetPos = new Vector3(Random.Range((int)this.transform.position.x , maxX + 1), Random.Range(minY, maxY + 1));

                    }

                    if (cropDetails.particalEffect)
                    {
                        for (int k = 0; k < cropDetails.ParticalEffectSystem.Length; k++)
                        {

                            EventHandler.CallUpPEInstantiateEvent(cropDetails.ParticalEffectSystem[k], new Vector3((int)this.transform.position.x, (int)this.transform.position.y, 0));

                        }
                    }


                    EventHandler.CallUpDropItemEvent(cropDetails.productedItemID[i], new Vector3((int)this.transform.position.x, (int)this.transform.position.y, 0), RandomTargetPos);

                }



            }
            Destroy(gameObject);


        }


    }

    
}
