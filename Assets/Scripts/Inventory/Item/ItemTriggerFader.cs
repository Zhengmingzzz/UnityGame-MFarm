using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTriggerFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();

        if (faders.Length > 0)
        {
            foreach (ItemFader I in faders)
            {
                I.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();

        if (faders.Length > 0)
        {
            foreach (ItemFader I in faders)
            {
                I.FadeIn();
            }
        }
    }
}
