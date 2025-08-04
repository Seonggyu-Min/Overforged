using System;
using System.Collections.Generic;
using Photon.Pun;

namespace SHG
{
  using Player = TestPlayer;

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
    public const string ITEM_ID_KEY = "ItemId";
    public const string PLAYER_NETWORK_ID_KEY = "PlayerNetworkId";
    public Item ItemToGive;
    public int PlayerNetworkId;

    public override string ToString()
    {
      return ($"[{nameof(ToolTransferArgs)}; {nameof(ItemToGive)}: {this.ItemToGive}; {nameof(PlayerNetworkId)}: {this.PlayerNetworkId};]");
    }

    public object ConvertToNetworkArguments()
    {
      Dictionary<string, object> args = new ();
      args[ITEM_ID_KEY] = this.ItemToGive != null ? this.ItemToGive.GetComponent<PhotonView>().ViewID: null;
      args[PLAYER_NETWORK_ID_KEY] = this.PlayerNetworkId;
      return (args);
    }
  }

  public struct ToolTransferResult 
  {
    public const string RECEIVED_ITEM_KEY = "RecievedItem";
    public const string IS_DOEN_KEY = "IsDone";
    public Item ReceivedItem;
    public bool IsDone;

    public override string ToString()
    {
      return ($"[{nameof(ToolTransferResult)}; {nameof(ReceivedItem)}: {this.ReceivedItem}; {nameof(IsDone)}: {this.IsDone};]");
    }

    public object ConvertToNetworkArguments()
    {
      Dictionary<string, object> args = new ();
      args[RECEIVED_ITEM_KEY] = this.ReceivedItem!= null ? this.ReceivedItem.GetComponent<PhotonView>().ViewID: null;
      args[IS_DOEN_KEY] = this.IsDone;
      return (args);
    }
  }

  public struct ToolWorkResult
  {
    public const string DURATION_TO_STAY_KEY = "DurationToStay";
    public const string IS_DONE_KEY = "IsDone";
    public Action Trigger;
    public float DurationToStay;
    public bool IsDone;
    
    public override string ToString()
    {
      return ($"[{nameof(ToolWorkResult)}; {nameof(Trigger)}: {this.Trigger}; {nameof(DurationToStay)}: {this.DurationToStay}; {nameof(IsDone)}: {this.IsDone};]");
    }

    public object ConvertToNetworkArguments()
    {
      Dictionary<string, object> args = new ();
      args[DURATION_TO_STAY_KEY] = this.DurationToStay;
      args[IS_DONE_KEY] = this.IsDone;
      return (args);
    }
  }

  public struct PlayerInteractArgs
  {
    public Item CurrentHoldingItem;
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
