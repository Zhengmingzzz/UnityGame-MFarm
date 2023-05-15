using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, cursorSeed, goods, UISprite;
    private RectTransform cursorCanvasTransfrom;
    private Image CursorImage;
    private Sprite currentSprite;

    private void Start()
    {
        cursorCanvasTransfrom = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        CursorImage = cursorCanvasTransfrom.GetChild(0).GetComponent<Image>();

        SetCursorImage(normal);
        currentSprite = normal;
    }

    private void Update()
    {
        if (CursorImage != null)
        {
            CursorImage.transform.position = Input.mousePosition;
            if (isInterActWithUI())
            {
                SetCursorImage(UISprite);
            }
            else
            {
                SetCursorImage(currentSprite);
            }



        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;

    }

    private void OnItemSelectEvent(ItemDetails itemDetail, bool isSelected)
    {
        if (!isSelected)
        {

            currentSprite = normal;
            
        }
        else
        {
            currentSprite = itemDetail.itemType switch 
            {
                ItemType.Seed =>cursorSeed,
                ItemType.Commodity=>goods,
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

}
