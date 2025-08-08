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
      if (args.ItemToGive != null) {
        if (args.ItemToGive is MaterialItem materialItem) {
          return ( materialItem.IsHot &&
            Array.IndexOf(
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
      // 로테이션 방식으로 계속 변경
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
