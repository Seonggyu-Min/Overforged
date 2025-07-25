using System;
using UnityEngine;

namespace SHG
{
  //TODO
  //재료 아이템에 효율을 차등 적용
  //온도에 따라 효율을 차등 적용
  public class Furnace : SmithingTool
  {
    const float TEMP_INCREASE_DELTA = 10f;
    const float TEMP_DECRESE_DELTA = -1f;
    const float MAX_TEMPARATURE = 1000f;
    const float MIN_TEMPARATURE = 20f;
    public override bool IsFinished => this.Progress >= 1.0f;

    public bool IsIgnited { get; private set; }
    public Action<bool> OnTurnIgnited;
    public Action OnFinished;
    public float Temparature { get; private set; }

    public Furnace(SmithingToolData data) : base(data)
    {
      this.Temparature = 0f;
    }

    protected override bool isPlayerMovable => true;

    protected override bool isRemamingTimeElapse => true;

    void OnTriggerIgnited()
    {
      this.OnTurnIgnited?.Invoke(this.IsIgnited);
    }

    public override void OnUpdate(float deltaTime)
    {
      bool wasFinished = this.IsFinished;
      if (this.IsIgnited) {
        base.OnUpdate(deltaTime);
      }
      this.Temparature += (this.IsIgnited ? 
        TEMP_INCREASE_DELTA: TEMP_DECRESE_DELTA) * deltaTime;
      this.Temparature = Math.Clamp(
        this.Temparature, MIN_TEMPARATURE, MAX_TEMPARATURE);
      if (!wasFinished && this.IsFinished) {
        this.OnFinished?.Invoke();
      }
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (this.HoldingItem == null);
      }
      else {
        return (this.ItemToReturn != null && this.IsFinished);
      }
    }

    public override bool CanWork()
    {
      return (!this.IsIgnited);
    }

    public override ToolWorkResult Work()
    {
      this.interactionToTrigger = InteractionType.Work;
      this.BeforeInteract?.Invoke(this);
      if (!this.IsIgnited) {
        this.IsIgnited = true;
      }
      else {
        // TODO: turn off fire?
      }
      return (
        this.ReturnWithEvent(
          new ToolWorkResult {
            Trigger = this.OnTriggered,
            DurationToStay = 0 
          }
      ));
    }
  }
}
