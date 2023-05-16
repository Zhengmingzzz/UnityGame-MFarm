using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag_SO",menuName = "Invention/InventoryBag_SO")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryType> itemList = new List<InventoryType>();
}
