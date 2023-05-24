using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour
{
    private Animator[] animators;
    [SerializeField]public List<AnimType> animType = new List<AnimType>();
    public SpriteRenderer holdItemSpriteRenderer;
    private Dictionary<string, Animator> animDic = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (Animator a in animators)
        {
            animDic.Add(a.name, a);
        }
    }


    private void OnEnable()
    {
        EventHandler.ItemSelectEvent += HoldItemInScene;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.HarvestCropOnPlayer += OnHarvestCropOnPlayer;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectEvent -= HoldItemInScene;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.HarvestCropOnPlayer -= OnHarvestCropOnPlayer;

    }

    private void OnHarvestCropOnPlayer(int seedID)
    {
        Sprite cropSprite = MFarm.Inventory.InventoryManager.Instance.GetItemDetailsByID(seedID).itemOnWorldSprite;
        if (holdItemSpriteRenderer.enabled == false)
        {
            StartCoroutine( ShowHarvestCrop(cropSprite));
        }
    }

    IEnumerator ShowHarvestCrop(Sprite cropSprite)
    {
        holdItemSpriteRenderer.sprite = cropSprite;
        holdItemSpriteRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);
        holdItemSpriteRenderer.enabled = false;
    }

    public void HoldItemInScene(ItemDetails itemdetails, bool isSelect)
    {
        //TODO:添加角色新动画时需在此设置
        NowState nowState = NowState.None;
        if (itemdetails != null)
        {
            nowState = itemdetails.itemType switch
            {
                ItemType.Seed => NowState.Carry,
                ItemType.Commodity => NowState.Carry,
                ItemType.Furniture => NowState.Carry,
                ItemType.HoeTool => NowState.Hoe,
                ItemType.WaterTool=>NowState.Water,
                ItemType.CollectionTool=>NowState.Harvest,
                _ => NowState.None
            };
        }


        if (nowState == NowState.Carry)
        {
            holdItemSpriteRenderer.sprite = itemdetails.itemOnWorldSprite;
            holdItemSpriteRenderer.enabled = isSelect;
        }
        else
        {
            holdItemSpriteRenderer.enabled = false;
        }
        
      
        if (!isSelect)
        {
            nowState = NowState.None;
        }

        SwitchAnim(nowState);
    }

    public void SwitchAnim(NowState nowstate)
    {
        
        foreach (AnimType a in animType)
        {
            if (a.nowState == nowstate)
            {
                animDic[a.bodyName.ToString()].runtimeAnimatorController = a.animator;
            }
        }
    }

    private void OnAfterLoadSceneEvent()
    {
        HoldItemInScene(null, false);
    }

}
