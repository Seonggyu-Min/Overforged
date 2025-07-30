using System;

namespace SHG
{
  public class QuenchingTool : SmithingTool
  {
    const float MAX_TEMPARATURE = 200f;
    const float MIN_TEMPARATURE = 30f;
    const float TEMP_DECRESE_MULTIPLYER = 0.5f;

    public override bool IsFinished => this.Progress >= 1.0f;
    public Action OnFinished;
    public float Temparature;
    protected override bool isPlayerMovable => false;
    protected override bool isRemamingTimeElapse => true;

    public QuenchingTool(SmithingToolData data) : base(data)
    {
      this.Temparature = MIN_TEMPARATURE;
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        if (args.ItemToGive is MaterialItem materialItem) {
          if (this.HoldingItem != null) {
            return (materialItem.IsHot);
          }
          return (
            Array.IndexOf(
              this.AllowedMaterials, materialItem.Variation) != -1);
        }
        else {
          return (false);
        }
      }
      return (this.HoldingItem != null && this.IsFinished);
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      //TODO: check item is hot
      if (args.ItemToGive != null) {
        this.Temparature = MAX_TEMPARATURE;
      }
      return (base.Transfer(args));
    }

    public override bool CanWork()
    {
      return (false);
    }

    public override ToolWorkResult Work()
    {
      #if UNITY_EDITOR
      throw (new ApplicationException($"{typeof(QuenchingTool)} is not requrired work"));
      #else
      return (new ToolWorkResult {});
      #endif
    }

    public override void OnUpdate(float deltaTime)
    {
      bool wasFinished = this.IsFinished;
      base.OnUpdate(deltaTime);
      if (this.Temparature > MAX_TEMPARATURE) {
        this.Temparature = Math.Max(
          this.Temparature * TEMP_DECRESE_MULTIPLYER, MIN_TEMPARATURE);
      }
      if (this.HoldingItem != null && 
        !wasFinished && this.IsFinished) {
        this.HoldingItem.Cool();
        this.OnFinished?.Invoke();
      }
    }
  }
}
