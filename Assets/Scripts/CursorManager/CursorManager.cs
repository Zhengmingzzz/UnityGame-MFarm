using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MFarm.CropPlant;
using UnityEngine.SceneManagement;
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
    /// <summary>
    /// 判断是否正在切换场景
    /// </summary>
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
            // 设置UI图标跟随鼠标位置
            CursorImage.transform.position = Input.mousePosition;
            // 如果和游戏UI互动，则更改UI图标
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

    /// <summary>
    /// 判断是否和游戏UI交互
    /// </summary>
    /// <returns></returns>
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
        // 如果在指定范围内（整个屏幕范围）没有鼠标，则将鼠标UI隐藏
        if (!new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;

        }

        // 根据鼠标位置得到相应的世界坐标(Vec3 float类型) 再根据世界坐标转为网格坐标(Vec3Int类型)
        mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);


        // 如果没有切换场景并且选择了物品的情况下 需要对鼠标UI做出调整
        if (!isTransition && isSelected)
        {
            // 存储鼠标点击的世界坐标下的tile信息
            TileDetail CheckTileDetailInfo = null;
            // 存储鼠标点击的世界坐标下的种子的信息
            Crop crop = MFarm.Map.GridMapManager.Instance.FindCropByMouseWorldPos(mouseWorldPos);
            
            // 我们希望点击整颗树都可以使鼠标合法，因此树要与其他农作物区别开
            if (selectedItemDetail.itemType == ItemType.ChopTool)
            {
                if (crop != null)
                {
                    // 因为树比较大，所以需要根据它的根部坐标计算
                    CheckTileDetailInfo = MFarm.Map.GridMapManager.Instance.getTileDetailByPos(SceneManager.GetActiveScene().name,new Vector3Int(crop.tileDetail.gridX, crop.tileDetail.gridY, 0));
                }
            }
            else
            {
                CheckTileDetailInfo = MFarm.Map.GridMapManager.Instance.getTileDetailByPos(SceneManager.GetActiveScene().name, mouseGridPos);
            }


            // 至此，选择的工具信息 鼠标对应的tile信息 对应的crop信息的获取 已经完成
            // 接下来会对选择工具范围进行判断
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
                    case ItemType.ReapTool:
                        if (MFarm.Map.GridMapManager.Instance.CheckReapItemValidInRadium(selectedItemDetail, mouseWorldPos))
                        {
                            mouseValid = true;
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
    /// <summary>
    /// 根据传入参数更改鼠标UI颜色
    /// </summary>
    /// <param name="isValid"></param>
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

    /// <summary>
    /// 根据itemDetail的范围和playerTransform变量（玩家位置）判断合法距离
    /// </summary>
    /// <param name="itemDetail"></param>
    /// <param name="TargetItemPos"></param>
    /// <returns></returns>
    private bool CheckUseRadiusValid(ItemDetails itemDetail,Vector3Int TargetItemPos)
    {
        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);
        if (Vector3Int.Distance(playerGridPos ,TargetItemPos) > itemDetail.itemUseRadius) 
        {
            return false;
        }
        return true;
    }


}
