using UnityEngine;

namespace SHG
{

  public class TestItemData: ScriptableObject
  {
    public string Name;
    public GameObject prefab;
    public Sprite Image;
  }

  public class TestItem
  {
    public TestItemData Data;
    public TestItem(TestItemData data)
    {
      this.Data = data;
    }
  }

  [CreateAssetMenu(menuName = "GameData/Test/MaterialItem")]
  public class TestMaterialItemData: TestItemData
  {
    public TestMaterialType Type;
    public TestMaterialItemData RefinedResult;
  }

  public class TestMaterialItem: TestItem
  {
    public TestMaterialType MaterialType;
    TestMaterialItemData refindResult;

    public TestMaterialItem GetRefinedResult()
    {
      return (new TestMaterialItem(this.refindResult));
    }
    public TestMaterialItem(TestMaterialItemData data): base(data)
    {
      this.MaterialType = data.Type;
      this.refindResult = data.RefinedResult;
    }
  }
}
