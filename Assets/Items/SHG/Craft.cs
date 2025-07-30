using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SHG
{

  public class Craft
  {
    public CraftData Data;
    public ProductItemData ProductItemData => this.Data.ProductItemData;
    public HashSet<MaterialItemData> Materials { get; private set; }
    public ProductItem CreateProduct(Vector3 position)
    {
      var gameObject = PhotonNetwork.Instantiate(
        prefabName: "ProductItem", 
        position: position,
        rotation: Quaternion.identity
        );
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
