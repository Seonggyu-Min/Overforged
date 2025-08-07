using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SHG
{
  using Variation = MaterialVariation;

  [Serializable]
  public struct ProductRecipe
  {
    public Part[] Parts;
    ProductType productType;
    OreType oreType;
    WoodType woodType;
    float timeStamp;
    public WoodType WoodType {
      get {
        if (this.Parts[0].WoodType != WoodType.None) {
          return (this.Parts[0].WoodType);
        }
          return (this.Parts[1].WoodType);
      }
    }
    public OreType OreType
    {
      get
      {
        if (this.Parts[0].OreType != OreType.None)
        {
          return (this.Parts[0].OreType);
        }
        return (this.Parts[1].OreType);
      }
    }

    public ProductRecipe(
      ProductType productType,
      OreType oreType,
      WoodType woodType,
      float timeStamp = 0f)
    {
      this.productType = productType;
      this.oreType = oreType;
      this.woodType = woodType;
      this.timeStamp = timeStamp;
      this.Parts = null;
      switch (this.productType) {
        case (ProductType.Bow):
        this.Parts = new Part[] { 
          new Part(Variation.BowBase, this.woodType),
          Part.String 
        };
        break;
        case (ProductType.Sword):
        this.Parts = new Part[] {
          new Part(Variation.Blade, this.oreType),
          new Part(Variation.Handle, this.woodType)
        };
        break;
        case (ProductType.Axe):
        this.Parts = new Part[] {
          new Part(Variation.AxeBlade, this.oreType),
          new Part(Variation.Handle, this.woodType)
        };
        break;
        case (ProductType.Hammer):
        this.Parts = new Part[] {
          new Part(Variation.HammerHead, this.oreType),
          new Part(Variation.Handle, this.woodType)
        };
        break;
        default: 
        #if UNITY_EDITOR
          Debug.LogError($"{nameof(ProductRecipe)} Unable to Get {nameof(Part)}");
        #endif
        break;
      }
    }

    public bool IsEqualTo(
      ProductType productType,
      OreType oreType,
      WoodType woodType)
    {
      return (
        this.productType == productType &&
        this.oreType == oreType &&
        this.woodType == woodType);
    }
  }
}
