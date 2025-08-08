using System;
using UnityEngine;

namespace SHG
{
  //TODO
  //재료 아이템에 효율을 차등 적용
  //온도에 따라 효율을 차등 적용
  public class Furnace : SmithingTool
  {
    const float TEMP_INCREASE_DELTA = 80f;
    const float TEMP_DECRESE_DELTA = -50f;
    public const float MAX_TEMPARATURE = 1000f;
    public const float MIN_TEMPARATURE = 20f;
    public override bool IsFinished => this.Progress >= 1.0f;
    public float NormalizedTemparature { get; private set; }

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
      if (this.HoldingMaterial != null != this.IsFinished) {
        this.RemainingTime -= deltaTime * this.NormalizedTemparature;
        if (this.RemainingTime < 0) {
          this.RemainingInteractionCount -= 1;
          this.RemainingTime = this.DefaultRequiredTime;
        }
      }
      this.Temparature += (this.IsIgnited ? 
        TEMP_INCREASE_DELTA: TEMP_DECRESE_DELTA) * deltaTime;
      this.Temparature = Math.Clamp(
        this.Temparature, MIN_TEMPARATURE, MAX_TEMPARATURE);
      this.NormalizedTemparature = Mathf.InverseLerp(
        Furnace.MIN_TEMPARATURE,
        Furnace.MAX_TEMPARATURE,
        this.Temparature);
      if (!wasFinished && this.IsFinished) {
        this.HoldingMaterial.ChangeToNext();
        this.OnFinished?.Invoke();
      }
    }

    public void TurnOff()
    {
      this.IsIgnited = false;
      this.InteractionToTrigger = InteractionType.Work; 
      this.OnTriggered();
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
      else {
        return (this.ItemToReturn != null);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      ToolTransferResult result = base.Transfer(args);
      if (this.HoldingMaterial != null && 
        (this.IsIgnited || this.NormalizedTemparature > 0.5f)) {
        this.HoldingMaterial.Heat();
      }
      this.ResetInteraction();
      return (result);
    }

    public override bool CanWork()
    {
      return (!this.IsIgnited);
    }

    public override ToolWorkResult Work()
    {
      this.InteractionToTrigger = InteractionType.Work;
      this.BeforeInteract?.Invoke(this);
      if (!this.IsIgnited) {
        this.IsIgnited = true;
        if (this.HoldingMaterial != null) {
          this.HoldingMaterial.Heat();
        }
      }
      else {
        // TODO: turn off fire?
      }
      return (
        this.ReturnWithEvent(
          new ToolWorkResult {
          Trigger = this.OnTriggered,
          DurationToStay = this.InteractionTime,
          IsDone = true
          }));
    }
  }
}
