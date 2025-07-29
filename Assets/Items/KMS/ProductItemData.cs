using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using UnityEditor.PackageManager.Requests;

[CreateAssetMenu(menuName = "ProductItemSO")]
public class ProductItemData : ItemData
{
    public ProductType productType;

    public List<MaterialItemData> MaterialList;



}

