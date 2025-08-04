using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class SmithingToolConstants 
  {
    SmithingToolList toolList;
    Dictionary<SmithingTool.ToolType, SmithingToolData> data;

    public static SmithingToolConstants Instance 
    {
      get {
        if (instance != null) {
          return (instance);
        }
        instance = new SmithingToolConstants();
        return (instance);
      }
    }
    static SmithingToolConstants instance;

    SmithingToolConstants()
    {
      this.toolList = Resources.Load<SmithingToolList>("SHG/SmithingToolList");
      this.data = new ();
      foreach (var tool in this.toolList.Tools) {
        this.data[tool.Type] = tool;
      }
    }

    public int GetWorkCount(SmithingTool.ToolType toolType)
    {
      if (this.data.TryGetValue(toolType, out SmithingToolData data)) {
        return (data.RequiredInteractCount);
      }
      return (1);
    }
  }
}
