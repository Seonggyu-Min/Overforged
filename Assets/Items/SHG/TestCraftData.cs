using UnityEngine;

namespace SHG
{
  [CreateAssetMenu (menuName = "GameData/Test/Craft")]
  public class TestCraftData : ScriptableObject
  {
    public ProductItemData ProductItemData;  
    public MaterialItemData[] Materials;
  }
}
