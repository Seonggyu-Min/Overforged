using System.Collections;
using System.Collections.Generic;
using MIN;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> list;

    public List<ItemData> productList;

    public Dictionary<int, ItemData> Dict;

    void OnEnable()
    {
        Dict = new();
        foreach (ItemData data in list)
        {
            Dict.Add(data.ID, data);
        }
    }




}

