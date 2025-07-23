using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public abstract class SmithingTool 
  {
    protected SmithingToolData Data;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
    }

    public abstract bool IsFinished();
    
  }
}

