using System.Collections;
using System.Collections.Generic;
using MIN;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> list;

    public List<ProductItemData> productList;

    public Dictionary<string, ProductItemData> ProductDict;

    void OnEnable()
    {
        ProductDict = new();
        foreach (ProductItemData data in productList)
        {
            ProductDict.Add(data.name, data);
        }
    }




}

