using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BotContext : MonoBehaviour
  {
    public static BotContext Instance;
    static BotContext instance;
    [SerializeField]
    RawMaterialBox[] materialBoxes;
    public RawMaterialBox[] MaterialBoxes => this.materialBoxes;
    Dictionary<int, List<SmithingToolComponent>> tools;

    public void AddRecipe(
      CraftData data,
      WoodType woodType,
      OreType oreType) {

    }

    public void RemoveRecipe(
      CraftData data,
      WoodType woodType,
      OreType oreType)
    {

    }
    
    void Awake()
    {
      if (instance == null) {
        instance = this;
        this.Init();
      }
      else {
        Destroy(this.gameObject);
      }
    }

    void Init()
    {
      this.tools = new ();
      
    }

    public T GetComponent<T>(int networkId, SmithingTool.ToolType toolType) where T: SmithingToolComponent
    {
      if (this.tools.TryGetValue(
          networkId, out List<SmithingToolComponent> botTools)) {
        switch (toolType) {
          case (SmithingTool.ToolType.Furnace):
            return (botTools.Find(tool => tool is FurnaceComponent) as T);
          case (SmithingTool.ToolType.Anvil):
            return (botTools.Find(tool => tool is AnvilComponent) as T);
          case (SmithingTool.ToolType.WoodTable):
          case (SmithingTool.ToolType.CraftTable):
            return (botTools.Find(tool => tool is TableComponent) as T);
          case (SmithingTool.ToolType.QuenchingTool):
            return (botTools.Find(tool => tool is QuenchingComponent) as T);
          default: 
            return (null as T);
        } 
      }
      else {
        return (null as T);
      }
    }

    void OnDestroy()
    {
      if (instance == this) {
        instance = null;
      }
    }
  }
}
