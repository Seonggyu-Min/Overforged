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

    public ProductRecipe(
      ProductType productType,
      OreType oreType,
      WoodType woodType
      )
    {
      switch (productType) {
        case (ProductType.Bow):
        this.Parts = new Part[] { 
          new Part(Variation.BowBase, woodType),
          Part.String 
        };
        break;
        case (ProductType.Sword):
        this.Parts = new Part[] {
          new Part(Variation.Blade, oreType),
          new Part(Variation.Handle, woodType)
        };
        break;
        case (ProductType.Axe):
        this.Parts = new Part[] {
          new Part(Variation.AxeBlade, oreType),
          new Part(Variation.Handle, woodType)
        };
        break;
        case (ProductType.Hammer):
        this.Parts = new Part[] {
          new Part(Variation.HammerHead, oreType),
          new Part(Variation.Handle, woodType)
        };
        break;
      }
      this.Parts = null;
      #if UNITY_EDITOR
      Debug.LogError($"{nameof(ProductRecipe)} Unable to Get {nameof(Part)}");
      #endif
    }
  }
}
