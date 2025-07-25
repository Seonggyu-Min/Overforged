using System;

namespace SHG
{

  //TODO
  // 재료 아이템 또는 플레이어의 상태에 따라 작업효율 차등 적용
  public class Anvil : SmithingTool
  {
    public override bool IsFinished => (this.RemainingInteractionCount < 1);

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;

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
      return (Array.IndexOf(this.AllowedMaterials, args.ItemToGive.Variation) != -1);
    }

    public override bool CanWork()
    {
      if (this.HoldingItem == null) {
        return (false);
      }
      // 로테이션 방식으로 계속 변경
      return (true);
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
