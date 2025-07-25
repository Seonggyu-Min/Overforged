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

    public Action<IInteractable> OnInteractionTriggered;
    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;
    protected override Item ItemToReturn => (
      this.HoldingItem != null ? this.HoldingItem.GetRefinedResult() : null);

    public WoodTable(SmithingToolData data): base(data)
    {
    }

    public override bool IsInteractable(PlayerInteractArgs args)
    {
      if (this.HoldingItem != null) {
        return (args.CurrentHoldingItem == null);
      }
      if (args.CurrentHoldingItem == null ||
        !(args.CurrentHoldingItem is MaterialItem materialItem) ||
        Array.IndexOf(this.AllowedMaterials, materialItem.MaterialType) == -1) {
        return (false);
      }
      return (true);
    }

    public override ToolInteractArgs Interact(PlayerInteractArgs args)
    {
#if UNITY_EDITOR
      if (!this.IsInteractable(args)) {
        throw (
          new ApplicationException(
            "player try to interact but not interactable"));
      }
#endif
      this.BeforeInteract?.Invoke(this, args);
      if (this.IsFinished) {
        return (this.ReturnWithEvent(this.ReturnItem()));
      }
      if (this.HoldingItem == null) {
        return (this.ReturnWithEvent(
            this.ReceiveMaterialItem(args.CurrentHoldingItem)));
      }
      return (this.ReturnWithEvent(
          this.DecreseInteractionCount(this.RemainingTime)));
    }

    protected override void OnTriggered(IInteractable interactable)
    {
      this.OnInteractionTriggered?.Invoke(interactable);
    }
  }
}
