using System;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;

    [Serializable]
    public class Anvil : SmithingTool, IInteractable
    {
      public override bool IsFinished => (this.RemainingInteractionCount < 1);

      public Action<IInteractable> OnInteractionTriggered;
      protected override bool isPlayerMovable => true;
      protected override bool isRemamingTimeElapse => false;
      protected override Item ItemToReturn => (
        this.HoldingItem != null ? this.HoldingItem.GetRefinedResult() : null);

      public Anvil(SmithingToolData data): base(data)
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
        //TODO
        //1.재료 아이템을 도구로 이동하거나 숨기고 아이콘 표시
        //2. 재료 아이템 또는 플레이어의 상태에 따라 남은 시간을 차등 적용
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
