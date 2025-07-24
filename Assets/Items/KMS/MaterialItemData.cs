using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;

namespace KMS
{
    [CreateAssetMenu(menuName = "MaterialItemSO")]
    public class MaterialItemData : MIN.ItemData
    {
        public MaterialType materialType;
        public MaterialItemData nextMaterial;
    }
    
}
