using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public enum ItemType
    {
        Trash,
        Ore,
        Bar,
        HeatedBlade,
        CooledBlade,
        Wood,
        Handle,
        Finished
    }

    public enum OreType
    {
        None,
        Iron,
        Copper,
        Gold
    }

    public enum WoodType
    {
        None,   // 없음
        Oak,    // 참나무
        Birch   // 자작나무
    }
}