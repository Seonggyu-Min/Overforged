using System.Collections.Generic;
using UnityEngine;

namespace SHG
{

  public class Craft
  {
    public CraftData Data;
    public ProductItemData ProductItemData => this.Data.ProductItemData;
    public HashSet<MaterialItemData> Materials { get; private set; }
    public ProductItem CreateProduct()
    {
      var gameObject = GameObject.Instantiate(
        Resources.Load<GameObject>("ProductItem"));
      var productItem = gameObject.GetComponent<ProductItem>();
      productItem.Data = this.ProductItemData;
      return (productItem);
    }

    public Craft(CraftData data)
    {
      this.Data = data; 
      this.Materials = new HashSet<MaterialItemData>(this.Data.Materials);
    }
  }
}
