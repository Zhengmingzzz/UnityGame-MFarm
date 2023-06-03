using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, cursorSeed, goods, UISprite, Furniture;
    private RectTransform cursorCanvasTransfrom;
    private Image CursorImage;
    private Sprite currentSprite;

    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private ItemDetails selectedItemDetail;
    private bool isSelected;
    private bool isTransition;
    private bool mouseValid;

    private Transform playerTransform;

    private bool RadiumValid;


    private void Awake()
    {
        playerTransform = FindObjectOfType<Player>().transform;

    }

    private void Start()
    {

        cursorCanvasTransfrom = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        CursorImage = cursorCanvasTransfrom.GetChild(0).GetComponent<Image>();

        SetCursorImage(normal);
        currentSprite = normal;

        mainCamera = GameObject.FindObjectOfType<Camera>();

    }

    private void Update()
    {
        if (CursorImage != null)
        {
            CursorImage.transform.position = Input.mousePosition;
            if (isInterActWithUI())
            {
                SetCursorValidColor(true);
                SetCursorImage(UISprite);
            }
            else
            {
                SetCursorImage(currentSprite);
                if (!isTransition)
                {
                    CheckCursorValid();
                }
                else
                {
                    SetCursorValidColor(true);
                    currentSprite = normal;
                    selectedItemDetail = null;
                    SetCursorImage(currentSprite);
                    isSelected = false;
                }
            }
            OnmouseClicked();
        }


    }

    private void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectEvent;
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;


    }

    private void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;

    }



    private void OnItemSelectEvent(ItemDetails itemDetail, bool isSelected)
    {
        this.isSelected = isSelected;

        
        if (!isSelected)
        {
            selectedItemDetail = null;
            currentSprite = normal;            
        }
        else
        {
            selectedItemDetail = itemDetail;
            currentSprite = itemDetail.itemType switch 
            {
                ItemType.Seed =>cursorSeed,
                ItemType.Commodity=>goods,
                ItemType.Furniture => Furniture,
                ItemType.HoeTool=>tool,
                ItemType.BreakTool => tool,
                ItemType.ChopTool => tool,
                ItemType.CollectionTool => tool,
                ItemType.ReapTool => tool,
                ItemType.WaterTool => tool,
                _=>normal,
            };
        }

    }

    private bool isInterActWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetCursorImage(Sprite sprite)
    {
        if (CursorImage != null)
        {
            CursorImage.sprite = sprite;
        }
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        isTransition = true;
        isSelected = false;
        selectedItemDetail = null;
    }



    private void OnAfterLoadSceneEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
        isTransition = false;
    }




    private void CheckCursorValid()
    {



        if (!new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;

        }


        mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        if (!isTransition && isSelected)
        {
            TileDetail CheckTileDetailInfo = null;
            Crop crop = MFarm.Map.GridMapManager.Instance.FindCropByMouseWorldPos(mouseWorldPos);
            if (selectedItemDetail.itemType == ItemType.ChopTool)
            {
                if (crop != null)
                {
                    CheckTileDetailInfo = MFarm.Map.GridMapManager.Instance.getTileDetailByPos(new Vector3Int(crop.tileDetail.gridX, crop.tileDetail.gridY, 0));
                }
            }
            else 
            {
                CheckTileDetailInfo = MFarm.Map.GridMapManager.Instance.getTileDetailByPos(mouseGridPos);
            }


            if (selectedItemDetail != null && CheckTileDetailInfo != null)
            {
                RadiumValid = CheckUseRadiusValid(selectedItemDetail, new Vector3Int(CheckTileDetailInfo.gridX, CheckTileDetailInfo.gridY, 0));

                mouseValid = false;
                CropDetails cropDetails = CropManager.Instance.GetCropDetailsByID(CheckTileDetailInfo.seedID);


                if (!RadiumValid)
                {
                    mouseValid = false;
                    goto PassSwitch;

                }

                //TODO:添加新的工具
                switch (selectedItemDetail.itemType)
                {
                    case ItemType.Commodity:
                        if (CheckTileDetailInfo.CanDropItem == true)
                        {
                            mouseValid = true ;
                        }
                        break;
                    case ItemType.Furniture:
                        if (CheckTileDetailInfo.CanPlaceFurniture == true)
                        {
                            mouseValid = true;
                        }
                        break;
                    case ItemType.Seed:
                        if (CheckTileDetailInfo.digSinceDay != -1 && CheckTileDetailInfo.seedID == -1)
                        {
                            mouseValid = true;
                            SetCursorValidColor(true);
                        }
                        else
                        {
                            SetCursorValidColor(false);
                        }
                        break;

                    case ItemType.HoeTool:
                        if (CheckTileDetailInfo.CanDig)
                        {
                            mouseValid = true;
                        }
                        break;
                    case ItemType.WaterTool:
                        if (CheckTileDetailInfo.digSinceDay != -1 && CheckTileDetailInfo.wateredSinceDay == -1)
                        {
                            mouseValid = true;
                        }
                        break;
                    case ItemType.ChopTool:
                    case ItemType.BreakTool:
                        if (crop != null && crop.cropDetails.CheckToolValid(selectedItemDetail.ItemID) && crop.canHarvest)
                        {
                            mouseValid = true;
                        }
                        break;
                    case ItemType.CollectionTool:
                        if (cropDetails != null && cropDetails.CheckToolValid(selectedItemDetail.ItemID)) 
                        {
                            if (cropDetails.TotalGlowthDays <= CheckTileDetailInfo.seedSinceDay)
                            {
                                mouseValid = true;
                            }
                        }
                        break;

                }
            PassSwitch:
                SetCursorValidColor(mouseValid);

            }
            else
            {
                mouseValid = false;
                SetCursorValidColor(mouseValid);
            }

        }
        else
        {
            currentSprite = normal;
            SetCursorImage(currentSprite);
            SetCursorValidColor(true);
            mouseValid = true;

        }
        


    }

    private void SetCursorValidColor(bool isValid)
    {
        if (isValid == true)
        {
            CursorImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            CursorImage.color = new Color(1, 0, 0, 0.4f);
        }
    }

    private void OnmouseClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedItemDetail != null && !isTransition && mouseValid)
            {
                EventHandler.CallUpMouseClickedEvent(mouseWorldPos, selectedItemDetail);
            }
        }
            
    }

    private bool CheckUseRadiusValid(ItemDetails itemDetail,Vector3Int TargetItemPos)
    {
        if (Vector3Int.Distance(new Vector3Int((int)playerTransform.position.x, (int)playerTransform.position.y, 0) ,TargetItemPos) > itemDetail.itemUseRadius) 
        {
            return false;
        }
        return true;
    }


}
