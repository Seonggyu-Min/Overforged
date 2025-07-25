using System;
using UnityEngine;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;

  //TODO
  //재료, 플레이어의 상태에 따라 작업효율 차등 적용
  public class WoodTable: SmithingTool
  {
    public override bool IsFinished => (this.RemainingInteractionCount < 1);

    public Action OnInteractionTriggered;
    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;
    protected override Item ItemToReturn => (
      this.HoldingItem != null ? this.HoldingItem.GetRefinedResult() : null);

    public WoodTable(SmithingToolData data): base(data)
    {
    }

    protected override void OnTriggered()
    {
      this.OnInteractionTriggered?.Invoke();
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        if (Array.IndexOf(this.AllowedMaterials, args.ItemToGive.MaterialType) != -1) {
          return (true);
        }
        return (false);
      }
      return (this.IsFinished && this.ItemToReturn != null);
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
      if (!this.IsFinished) {
        return (this.ReturnWithEvent(
          this.DecreseInteractionCount(
            this.RemainingTime)));
      }
      return (this.ReturnWithEvent(
          this.ChangeMaterial(this.RemainingTime)));
    }
  }
}
