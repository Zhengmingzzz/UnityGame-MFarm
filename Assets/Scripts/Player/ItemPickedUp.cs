using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class ItemPickedUp : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            RenderItem item = collision.GetComponent<RenderItem>();


            if (item != null && item.itemDetails.canPickedUp == true)
            {

                InventoryManager.Instance.PickedUpItem(item, true);
            }
        }

    }
}