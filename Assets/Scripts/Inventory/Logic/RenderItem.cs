using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.CropPlant;

namespace MFarm.Inventory
{
    public class RenderItem : MonoBehaviour
    {
        public int ItemID;
        private SpriteRenderer spriteRender;
        private BoxCollider2D coll;
        public ItemDetails itemDetails;


        private void Awake()
        {
            spriteRender = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();

        }

        private void Start()
        {
            if (ItemID != 0)
                Init();
        }

        public void Init()
        {
            itemDetails = InventoryManager.Instance.GetItemDetailsByID(ItemID);

            if (itemDetails != null)
            {
                spriteRender.sprite = itemDetails.itemOnWorldSprite == null ? null : itemDetails.itemOnWorldSprite;

                coll.size = spriteRender.bounds.size;

                coll.offset = spriteRender.sprite.bounds.center;
            }



            if (itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>().InitReapItem(ItemID);

            }
        }


    }
}

