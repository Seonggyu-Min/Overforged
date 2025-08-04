using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using Variation = MaterialVariation;

  [Serializable]
  public struct Part 
  {
    public static readonly Part String = new Part { 
      Variation = Variation.String
    };
    public Variation Variation;
    public OreType OreType;
    public WoodType WoodType;

    public Part(
      Variation variation,
      OreType oreType
      ) {
      this.Variation = variation;
      this.OreType = oreType;
      this.WoodType = (WoodType)0;
    }

    public Part(
      Variation variation,
      WoodType woodType)
    {
      this.Variation = variation;
      this.WoodType = woodType;
      this.OreType = (OreType)0;
    }

    public bool TryGetPreviousPart(out Part prevPart)
    {
      switch (this.Variation) {
        case (Variation.Blade):
          prevPart = (new Part(Variation.Bar, this.OreType));
          return (true);
        case (Variation.AxeBlade):
          prevPart = (new Part(Variation.Blade, this.OreType));
          return (true);
        case (Variation.HammerHead):
          prevPart = new Part(Variation.AxeBlade, this.OreType);
          return (true);
        case (Variation.Handle):
          prevPart = new Part(Variation.Wood, this.WoodType);
          return (true);
        case (Variation.BowBase):
          prevPart = new Part(Variation.Handle, this.WoodType);
          return (true);
      }
      prevPart = new Part();
      return (false);
    }

    public Nullable<RawMaterial> TryGetRawMaterial()
    {
      switch (this.Variation) {
        case (Variation.Bar):
        case (Variation.Blade):
        case (Variation.HammerHead):
        case (Variation.AxeBlade):
          return (new RawMaterial(MaterialType.Mineral, this.OreType));
        case (Variation.Handle):
        case (Variation.BowBase):
          return (new RawMaterial(MaterialType.Wooden, this.WoodType));
        case (Variation.String):
          return (RawMaterial.String);
      }
      #if UNITY_EDITOR
      Debug.LogError($"{nameof(Part)}: Fail to {nameof(TryGetRawMaterial)}");
      #endif
      return (null);
    }

    public Nullable<SmithingTool.ToolType> GetToolType()
    {
      switch (this.Variation) {
        case (Variation.Bar):
          return (SmithingTool.ToolType.Furnace);
        case (Variation.Blade):
        case (Variation.AxeBlade):
        case (Variation.HammerHead):
          return (SmithingTool.ToolType.Anvil);
        case (Variation.Handle):
        case (Variation.BowBase):
          return (SmithingTool.ToolType.WoodTable);
      }
      #if UNITY_EDITOR
      Debug.LogError($"{nameof(GetToolType)}: Fail to get Tool Type for {this.Variation}");
      #endif
      return (null);
    }

    public bool IsEqual(Item item)
    {
      if (item is MaterialItem materialItem &&
        materialItem.Variation == this.Variation) {
        switch (this.Variation) {
          case (Variation.Ore):
          case (Variation.Bar):
          case (Variation.Blade):
          case (Variation.AxeBlade):
          case (Variation.HammerHead):
            return (this.OreType == materialItem.Ore);
          case (Variation.BowBase):
          case (Variation.Handle):
            return (this.WoodType == materialItem.Wood);
          case (Variation.String):
            return (true);
        }
      }
      return (false);
    }
  }
}
