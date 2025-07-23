
namespace SHG
{
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

  public abstract class SmithingTool 
  {
    public bool IsFinished { get; protected set; }
    public MaterialItem HoldingItem { get; protected set; }
    public MaterialType[] AllowedMaterials => this.Data.AllowdMaterials;

    protected SmithingToolData Data;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
    }
  }
}

