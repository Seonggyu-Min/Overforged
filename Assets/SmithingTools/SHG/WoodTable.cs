using System;

namespace SHG
{

  //TODO
  //재료, 플레이어의 상태에 따라 작업효율 차등 적용
  public class WoodTable: SmithingTool
  {
    public override bool IsFinished => (this.RemainingInteractionCount < 1);

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;

    public WoodTable(SmithingToolData data): base(data)
    {
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        if (args.ItemToGive is MaterialItem materialItem) {
          return (Array.IndexOf(
              this.AllowedMaterials, materialItem.Variation) != -1);
        }
        else {
          return (false);
        }
      }
      return (this.ItemToReturn != null);
    }

    public override bool CanWork()
    {
      if (this.HoldingMaterial == null) {
        return (false);
      }
      if (this.IsFinished) {
        return (false);
      }
      return (true);
    }

    public override ToolWorkResult Work(bool fromNetwork = false)
    {
      this.InteractionToTrigger = InteractionType.Work;
      this.BeforeInteract?.Invoke(this);  
      ToolWorkResult result = new ToolWorkResult {};
      if (!this.IsFinished) {
        result = this.DecreseInteractionCount(this.InteractionTime);
      }
      if (this.IsFinished) {
        result = this.ChangeMaterial(this.InteractionTime);
      }
      return (this.ReturnWithEvent(result));
    }
  }
}
