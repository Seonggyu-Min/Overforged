
namespace SHG
{
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

    public override string ToString()
    {
      return ($"[{nameof(TestMaterialItem)}; [Name: {this.Data.Name}]]");
    }
  }
}
