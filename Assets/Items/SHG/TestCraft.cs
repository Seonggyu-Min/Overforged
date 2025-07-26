using System.Collections.Generic;

namespace SHG
{
  using CraftData = TestCraftData;

  public class TestCraft
  {
    public CraftData Data;
    public ProductItemData ProductItemData => this.Data.ProductItemData;
    public HashSet<MaterialItemData> Materials { get; private set; }

    public TestCraft(CraftData data)
    {
      this.Data = data; 
      this.Materials = new HashSet<MaterialItemData>(this.Data.Materials);
    }
  }
}
