using System;
using System.Collections;

namespace SHG
{
  using Player = TestPlayer;
  using MaterialItem = TestMaterialItem;
  using Item = TestItem;

  public interface IInteractableTool
  {
    public bool CanTransferItem(ToolTransferArgs args);
    public ToolTransferResult Transfer(ToolTransferArgs args);
    public bool CanWork();
    public ToolWorkResult Work();
  }

  public interface IInteractable
  {
    public bool IsInteractable(PlayerInteractArgs args);
    public ToolInteractArgs Interact(PlayerInteractArgs args);
  }

  public struct ToolTransferArgs
  {
    public MaterialItem ItemToGive;
    public int PlayerNetworkId;

    public override string ToString()
    {
      return ($"[{nameof(ToolTransferArgs)}; {nameof(ItemToGive)}: {this.ItemToGive}; {nameof(PlayerNetworkId)}: {this.PlayerNetworkId};]");
    }
  }

  public struct ToolTransferResult 
  {
    public Item ReceivedItem;

    public override string ToString()
    {
      return ($"[{nameof(ToolTransferResult)}; {nameof(ReceivedItem)}: {this.ReceivedItem};]");
    }
  }

  public struct ToolWorkResult
  {
    public Action Trigger;
    public float DurationToStay;
    
    public override string ToString()
    {
      return ($"[{nameof(ToolWorkResult)}; {nameof(Trigger)}: {this.Trigger}; {nameof(DurationToStay)}: {this.DurationToStay};]");
    }
  }

  public struct PlayerInteractArgs
  {
    public MaterialItem CurrentHoldingItem;
    public int PlayerNetworkId;

    public override string ToString()
    {
      return ($"[{nameof(PlayerInteractArgs)}; {nameof(CurrentHoldingItem)}: {this.CurrentHoldingItem}; {nameof(PlayerNetworkId)}: {this.PlayerNetworkId};]");
    }
  }

  public struct ToolInteractArgs
  {
    public Item ReceivedItem;
    public float DurationToPlayerStay;
    public bool IsMaterialItemTaken;
    public Action<IInteractable> OnTrigger;

    public override string ToString()
    {
      return ($"[{nameof(ToolInteractArgs)}; {nameof(ReceivedItem)}: {this.ReceivedItem}; {nameof(DurationToPlayerStay)}: {DurationToPlayerStay}; {nameof(IsMaterialItemTaken)}: {this.IsMaterialItemTaken}]");
    }
  }
}
