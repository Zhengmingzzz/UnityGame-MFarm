using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetailList_SO", menuName = "Invention/ItemDetailList_SO")]
public class ItemDetailList_SO : ScriptableObject
{
    public List<ItemDetails> ItemDetailsList = new List<ItemDetails>();
}
