using System;
using UnityEditor;
using UnityEngine;

namespace SHG
{
  public class Furnace : SmithingTool
  {
    public bool IsIgnited { get; private set; }
    public Action<bool> OnTurnIgnited;

    public Furnace(SmithingToolData data) : base(data)
    {
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

    public override bool IsInteractable(PlayerInteractArgs args)
    {
      if (!this.IsIgnited)
      {
        return (true);
      }
      if (this.IsFinished)
      {
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
      Debug.Log($"{nameof(OnTriggered)}  in {nameof(Furnace)}");
    }
  }
}