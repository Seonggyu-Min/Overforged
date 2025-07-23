using System;

namespace SHG
{
  using MaterialItem = TestMaterialItem;

    public class Anvil : SmithingTool, IInteractable
    {
      public override bool IsFinished => (
        this.RemainingTime <= 0 && this.RemainingInteractionCount < 1);

      protected override bool isPlayerMovable => true;
      protected override bool isRemamingTimeElapse => false;

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
          Array.IndexOf(this.AllowedMaterials, materialItem) == -1) {
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
        #else
        if(!(args.CurrentHoldingItem is MaterialItem materialItem)) {
          return (new ToolInteractArgs {});
        }
        #endif
        this.BeforeInteract?.Invoke(this, args);
        if (this.IsFinished) {
          return (this.ReturnWithEvent(this.ReturnItem()));
        }
        //TODO
        //1.재료 아이템을 도구로 이동하거나 숨기고 아이콘 표시
        //2. 재료 아이템 또는 플레이어의 상태에 따라 남은 시간을 차등 적용
        //3.UI에 변화를 알려주는 기능
        if (this.HoldingItem == null) {
          return (this.ReturnWithEvent(
              this.ReceiveMaterialItem(args.CurrentHoldingItem)));
        }
        return (this.ReturnWithEvent(
            this.DecreseInteractionCount()));
      }

      ToolInteractArgs DecreseInteractionCount()
      {
        this.RemainingInteractionCount -= 1;
        return (new ToolInteractArgs {
          ReceivedItem = null,
          DurationToPlayerStay = this.RemainingTime
        });
      }

      ToolInteractArgs ReceiveMaterialItem(MaterialItem materialItem)
      {
        this.HoldingItem = materialItem;  
        this.isInteracting = true;
        ToolInteractArgs result = new ToolInteractArgs {
          ReceivedItem = null,
          DurationToPlayerStay = 0
        };
        return (result);
      }

      ToolInteractArgs ReturnItem()
      {
        var item = this.ItemToReturn;
        this.ResetInteraction();
        return (new ToolInteractArgs {
          ReceivedItem = item,
          DurationToPlayerStay = 0
        });
      }

      ToolInteractArgs ReturnWithEvent(in ToolInteractArgs result)
      {
        this.AfterInteract?.Invoke(this, result);
        return (result);
      }

      void ResetInteraction()
      {
        this.RemainingTime = this.DefaultRequiredTime;
        this.RemainingInteractionCount = this.DefaultRequiredInteractCount;
        this.HoldingItem = null;
        this.isInteracting = false;
      }
    }
}
