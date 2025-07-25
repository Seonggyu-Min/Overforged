using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;


    [CreateAssetMenu(menuName = "MaterialItemSO")]
    public class MaterialItemData : ItemData
    {
        public MaterialVariation materialVariation;
        public MaterialType materialType;
        public MaterialItemData nextMaterial;
    }
    
