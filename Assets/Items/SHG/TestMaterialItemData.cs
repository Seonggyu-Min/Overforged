using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [CreateAssetMenu(menuName = "GameData/Test/MaterialItem")]
  public class TestMaterialItemData: ItemData
  {
    public TestMaterialType Type;
    public TestMaterialItemData RefinedResult;
  }
}
