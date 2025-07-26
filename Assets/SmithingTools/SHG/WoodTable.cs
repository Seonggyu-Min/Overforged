using System;
using UnityEngine;

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
        return (Array.IndexOf(this.AllowedMaterials, args.ItemToGive.Variation) != -1);
      }
      return (this.ItemToReturn != null);
    }

    public override bool CanWork()
    {
      if (this.HoldingItem == null) {
        return (false);
      }
      if (this.IsFinished) {
        return (false);
      }
      return (true);
    }

    public override ToolWorkResult Work()
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
