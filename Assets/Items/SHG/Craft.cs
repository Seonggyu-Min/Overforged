using System.Collections.Generic;

namespace SHG
{

  public class Craft
  {
    public CraftData Data;
    public ProductItemData ProductItemData => this.Data.ProductItemData;
    public HashSet<MaterialItemData> Materials { get; private set; }

    public Craft(CraftData data)
    {
      this.Data = data; 
      this.Materials = new HashSet<MaterialItemData>(this.Data.Materials);
    }
  }
}
