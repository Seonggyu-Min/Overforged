using UnityEngine;

namespace SHG
{

  public class TestItemData: ScriptableObject
  {
    public string Name;
    public GameObject prefab;
    public Sprite Image;
  }

  [CreateAssetMenu(menuName = "GameData/Test/MaterialItem")]
  public class TestMaterialItemData: TestItemData
  {
    public TestMaterialType Type;
  }

  public class TestMaterialItem
  {
    public TestMaterialItemData Data;
    public TestMaterialType MaterialType => this.Data.Type;

    public TestMaterialItem(TestMaterialItemData data)
    {
      this.Data = data;
    }
  }
}
