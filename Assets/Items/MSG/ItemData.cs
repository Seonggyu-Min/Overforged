using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    [CreateAssetMenu(menuName = "ItemSO")]
    public class ItemData : ScriptableObject
    {
        public int ID;
        public string Name;

        //public ItemType ItemType;
        //public OreType OreType;
        //public WoodType WoodType;


        //public bool NeedTongs;        // 집게가 필요한지 여부
        public Sprite Image;
        public GameObject ItemPrefab; // 현재 아이템 프리팹
        //public ItemData NextItem;     // 다음 아이템 SO

        // public float TimeNeeded;

        //public InteractionObject InterationObj; // 모루 등

        // public ItemData FinishedItem;
        public ItemType CombineObjType;
    }
}
