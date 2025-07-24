using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHG;

namespace KMS
{
    public class ProductItem : Item
    {
        protected override void InitItemData(TestItemData itemdata)
        {
            Instantiate(data.prefab, transform);

        }
    }
}
