using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public struct RawMaterial
  {
    public static readonly RawMaterial String = new RawMaterial {
      MaterialType = MaterialType.None
    };
    public MaterialType MaterialType;
    public WoodType WoodType;
    public OreType OreType;

    public RawMaterial(
      MaterialType materialType,
      WoodType woodType)
    {
      this.MaterialType = materialType;
      this.WoodType = woodType;
      this.OreType = (OreType)0;
    }

    public RawMaterial(
      MaterialType materialType,
      OreType oreType)
    {
      this.MaterialType = materialType;
      this.OreType = oreType;
      this.WoodType = (WoodType)0;
    }
  }
}
