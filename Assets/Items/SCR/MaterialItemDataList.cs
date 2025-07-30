using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCR
{
    [CreateAssetMenu(menuName = "Item/ItemSOList")]
    public class MaterialItemDataList : ScriptableObject
    {
        public List<ItemInfo> itemInfo;
    }

    [Serializable]
    public struct ItemInfo
    {
        public string Name;
        public ItemData itemData;
    }
}

