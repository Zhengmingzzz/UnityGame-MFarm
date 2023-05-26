using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : Singleton<CropManager>
{
    public CropDetails_SO cropDetails_SO;

    private Transform CropParent;

    private Season currentSeason = Season.春天;
    private void OnEnable()
    {
        EventHandler.PlantEvent += OnPlantEvent;
        EventHandler.UpdataGameDayEvent += OnUpdataGameDayEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
    }
    private void OnDisable()
    {
        EventHandler.PlantEvent -= OnPlantEvent;
        EventHandler.UpdataGameDayEvent -= OnUpdataGameDayEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;

    }



    private void OnPlantEvent(int seedID, TileDetail tileDetail)
    {
        CropDetails currentCropDetails = GetCropDetailsByID(seedID);
        if (currentCropDetails != null && CheckSeasonValid(currentCropDetails))
        {
            //第一次种
            if (tileDetail.seedSinceDay == -1)
            {
                tileDetail.seedSinceDay = 0;
                tileDetail.seedID = currentCropDetails.seedID;
                DisplayPlant(currentCropDetails, tileDetail);
            }
            //不是第一次种
            else
            {
                DisplayPlant(currentCropDetails, tileDetail);
            }
             
        }
    }

    private void OnUpdataGameDayEvent(int day, Season season)
    {
        currentSeason = season;
    }


    private void OnAfterLoadSceneEvent()
    {
        CropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
    }

    public CropDetails GetCropDetailsByID(int seedID)
    {
        foreach (var i in cropDetails_SO.CropDetailsList)
        {
            if (i.seedID == seedID)
            {
                return i;
            }
        }
        return cropDetails_SO.CropDetailsList.Find(c => c.seedID == seedID);
    }

    private bool CheckSeasonValid(CropDetails cropDetails)
    {
        foreach (Season season in cropDetails.season)
        {
            if (currentSeason == season)
            {
                return true;
            }
        }
        return false;
    }

    

    public void DisplayPlant(CropDetails cropDetails, TileDetail tileDetails)
    {
        int currentStage = GetCropGlowthStage(cropDetails, tileDetails);

        GameObject cropPrefab = cropDetails.seedPrefabs[currentStage];
        Vector3 CropPos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
        Sprite cropSprite = cropDetails.seedSprite[currentStage];



        GameObject cropInstance = Instantiate(cropPrefab, CropPos, Quaternion.identity, CropParent);
        cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;


        cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
        cropInstance.GetComponent<Crop>().tileDetail = tileDetails;

    }


    private int GetCropGlowthStage(CropDetails cropDetails, TileDetail tileDetail)
    {
        int cropGlowthStageAmount = cropDetails.growthDays.Length - 1;
        int currentStage = cropGlowthStageAmount;
        int dayCounter = tileDetail.seedSinceDay;

        if (dayCounter >= cropDetails.TotalGlowthDays)
        {
            return cropGlowthStageAmount;
        }
        for (int i = 0; i < cropGlowthStageAmount; i++)
        {
            dayCounter -= cropDetails.growthDays[i];
            if (dayCounter <= 0)
            {
                currentStage = i;
                break;
            }
        }
        if (currentStage == cropGlowthStageAmount)
        {

            currentStage--;
            if (currentStage == -1)
            {
                currentStage = 0;
            }
        }
        return currentStage;


    }
}
