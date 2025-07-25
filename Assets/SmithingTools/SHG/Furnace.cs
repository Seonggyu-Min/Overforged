using System;
using UnityEngine;

namespace SHG
{
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
    public Action<IInteractable> OnInteractionTrigged;

    public Furnace(SmithingToolData data) : base(data)
    {
      this.Temparature = 0f;
    }

    protected override bool isPlayerMovable => true;

    protected override bool isRemamingTimeElapse => true;

    protected override TestItem ItemToReturn => (
      this.HoldingItem != null ? this.HoldingItem.GetRefinedResult(): null);

    public override ToolInteractArgs Interact(PlayerInteractArgs args)
    {
      #if UNITY_EDITOR
      if (!this.IsInteractable(args)) {
        throw (new ApplicationException($"Player is not interactable with {nameof(Furnace)}"));
      }
      #endif
      this.BeforeInteract?.Invoke(this, args);
      if (!this.IsIgnited) {
        this.IsIgnited = true;
        return (this.ReturnWithEvent(
            new ToolInteractArgs {
              ReceivedItem = null,
              DurationToPlayerStay = 0f,
              IsMaterialItemTaken = false,
              OnTrigger = this.OnInteractionTrigged,
            }));
      }
      if (args.CurrentHoldingItem != null) {
          return (this.ReturnWithEvent(
            this.ReceiveMaterialItem(args.CurrentHoldingItem)));
      }
      if (this.IsFinished) {
        return (this.ReturnWithEvent( this.ReturnItem()));
      }
      return (new ToolInteractArgs());
    }

    ToolInteractArgs TurnIgnited()
    {
      this.IsIgnited = !this.IsIgnited;

      return (new ToolInteractArgs {
        ReceivedItem = null,
        DurationToPlayerStay = 0,
        IsMaterialItemTaken = false,
        OnTrigger = this.OnTriggerIgnited
      });
    }

    void OnTriggerIgnited(IInteractable interactable)
    {
      if (System.Object.ReferenceEquals(this, interactable)) {
        this.OnTurnIgnited?.Invoke(this.IsIgnited);
      }
    }

    public override void OnUpdate(float deltaTime)
    {
      bool wasFinished = this.IsFinished;
      base.OnUpdate(deltaTime);
      this.Temparature += (this.IsIgnited ? 
        TEMP_INCREASE_DELTA: TEMP_DECRESE_DELTA) * deltaTime;
      this.Temparature = Math.Clamp(
        this.Temparature, MIN_TEMPARATURE, MAX_TEMPARATURE);
      if (!wasFinished && this.IsFinished) {
        this.OnFinished?.Invoke();
      }
    }

    public override bool IsInteractable(PlayerInteractArgs args)
    {
      if (!this.IsIgnited) {
        return (true);
      }
      if (this.IsFinished) {
        Debug.Log($"IsFinished");
        return (args.CurrentHoldingItem == null);
      }
      if (this.HoldingItem == null && args.CurrentHoldingItem != null)
      {
        return (true);
      }
      return (false);
    }

    protected override void OnTriggered(IInteractable interactable)
    {
      this.OnInteractionTrigged?.Invoke(interactable);
    }
  }
}
