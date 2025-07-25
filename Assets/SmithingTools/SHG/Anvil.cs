using System;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;

  //TODO
  // 재료 아이템 또는 플레이어의 상태에 따라 작업효율 차등 적용
  public class Anvil : SmithingTool
  {
    public override bool IsFinished => (this.RemainingInteractionCount < 1);

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;
    protected override Item ItemToReturn => (
      this.HoldingItem != null ? this.HoldingItem.GetRefinedResult() : null);

    public Anvil(SmithingToolData data): base(data)
    {
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive == null) {
        return (this.HoldingItem != null && this.IsFinished);
      }
      if (this.ItemToReturn != null) {
        return (false);
      }
      return (Array.IndexOf(this.AllowedMaterials, args.ItemToGive.MaterialType) != -1);
    }

    public override bool CanWork()
    {
      if (this.HoldingItem == null) {
        return (false);
      }
      if (!this.IsFinished) {
        return (true);
      }
      else {
        var nextItem = this.HoldingItem.GetRefinedResult();
        return (nextItem != null);
      }
    }

    public override ToolWorkResult Work()
    {
      this.interactionToTrigger = InteractionType.Work;
      this.BeforeInteract?.Invoke(this);  
      if (!this.IsFinished) {
        return (this.ReturnWithEvent(
            this.DecreseInteractionCount(this.RemainingTime)));
      }
      else {
        return (this.ReturnWithEvent(
          this.ChangeMaterial(this.RemainingTime)));
      }
    }
  }
}
