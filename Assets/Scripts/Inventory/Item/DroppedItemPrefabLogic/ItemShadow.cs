using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShadow : MonoBehaviour
{
    public Sprite itemSprite;
    private SpriteRenderer ShadowSpriteRenderer;



    private void Start()
    {
        ShadowSpriteRenderer = GetComponent<SpriteRenderer>();
        itemSprite = this.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().sprite;


        if (ShadowSpriteRenderer != null && itemSprite != null)
        {
            ShadowSpriteRenderer.sprite = itemSprite;
            ShadowSpriteRenderer.color = new Color(0, 0, 0, 0.3f);
        }
        
    }

}
