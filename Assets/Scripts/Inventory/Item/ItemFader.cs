using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemFader : MonoBehaviour
{
    SpriteRenderer thisSpriteRender;
    private void Awake()
    {
        thisSpriteRender = this.GetComponent<SpriteRenderer>();
    }
    public void FadeOut()
    {
        Color color = new Color(1f, 1f, 1f, Settings.targetAlpah);
        thisSpriteRender.DOColor(color, Settings.itemFadeDuration);
    }

    public void FadeIn()
    {
        Color color = new Color(1f, 1f, 1f, 1f);
        thisSpriteRender.DOColor(color, Settings.itemFadeDuration);
    }
}
