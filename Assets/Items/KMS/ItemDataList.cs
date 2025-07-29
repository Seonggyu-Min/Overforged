using System.Collections;
using System.Collections.Generic;
using MIN;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> list;

    public List<ItemData> productList;

    public Dictionary<string, ItemData> ProductDict;

    void OnEnable()
    {
        ProductDict = new();
        foreach (ItemData data in productList)
        {
            ProductDict.Add(data.name, data);
        }
    }




}

