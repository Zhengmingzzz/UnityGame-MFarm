using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MFarm.Inventory
{

    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI => GetComponent<SlotUI>();

        private void Update()
        {

            if (Input.GetKeyDown(key))
            {
                if (slotUI != null && slotUI.itemDetail != null)
                {
                    slotUI.isSelect = !slotUI.isSelect;
                    if (slotUI.isSelect)
                    {
                        slotUI.inventoryUI.UpdataSlotHightLight(slotUI.SlotIndex);
                    }
                    else
                        slotUI.inventoryUI.UpdataSlotHightLight(-1);

                    EventHandler.CallUpItemSelectEvent(slotUI.itemDetail, slotUI.isSelect);


                }



            }
        }
    }
}