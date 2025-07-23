using System;
using System.Collections;

namespace SHG
{
  using Player = TestPlayer;
  using MaterialItem = TestMaterialItem;
  using Item = TestItem;

  public interface IInteractable
  {
    public bool IsInteractable(PlayerInteractArgs args);
    public ToolInteractArgs Interact(PlayerInteractArgs args);
  }

  public struct PlayerInteractArgs
  {
    public MaterialItem CurrentHoldingItem;
    public int PlayerNetworkId;
    public Action OnCancel;

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

    public override string ToString()
    {
      return ($"[{nameof(ToolInteractArgs)}; {nameof(ReceivedItem)}: {this.ReceivedItem}; {nameof(DurationToPlayerStay)}: {DurationToPlayerStay}; {nameof(IsMaterialItemTaken)}: {this.IsMaterialItemTaken}]");
    }
  }
}
