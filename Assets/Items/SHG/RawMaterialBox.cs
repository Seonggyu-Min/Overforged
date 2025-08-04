using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using BoxComponent = SCR.BoxComponent;
  [Serializable]
  public struct RawMaterialBox
  {
    public RawMaterial Material;
    public BoxComponent Box;
  }
}
