using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public class DropOffTableComponent :SmithingToolComponent 
  {

    [SerializeField] [Required] 
    Transform itemPoint;
    [SerializeField] [Required]
    MeshRenderer model;
    [SerializeField] [Required]
    SmithingToolData tableData; 
    protected override SmithingTool tool => this.table;
    protected override Transform materialPoint => this.itemPoint;
    protected override ISmithingToolEffecter effecter => null;
    protected override bool isProgressUsed => false;
    DropOffTable table;

    public System.Action<ToolTransferResult> OnDropOffTransfered;

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      return (this.table.CanTransferItem(args));
    }

    public override bool CanWork()
    {
      return (this.table.CanWork());
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args, bool fromNetwork = false)
    {
      if (this.IsOwner) {
        ToolTransferResult result = (this.tool.Transfer(args));
        if (args.ItemToGive != null) {
          args.ItemToGive.gameObject.transform.SetParent(this.transform);
          args.ItemToGive.gameObject.transform.position = this.itemPoint.position;
          args.ItemToGive.gameObject.transform.rotation = this.itemPoint.rotation;
        }
        this.NetworkSynchronizer.SendRpc(
          sceneId: this.SceneId,
          method: nameof(Transfer),
          args: new object[] {
            args.ConvertToNetworkArguments(),
            result.ConvertToNetworkArguments()
          });
        this.OnDropOffTransfered?.Invoke(result);
        return (result);
      }
      else {
        ToolTransferResult result = new ToolTransferResult {
          ReceivedItem = this.table.HoldingItem
        };
        this.NetworkSynchronizer.SendRpcToMaster(
          sceneId: this.SceneId,
          method: nameof(OnItemTransferRequest),
          args: new object[] {
            args.ConvertToNetworkArguments(),
            result.ConvertToNetworkArguments()
          });
          this.OnDropOffTransfered?.Invoke(result);
        return (result);
      }
    }

    public override void OnRpc(string method, float latencyInSeconds, object[] args = null)
    {
      switch (method)
      {
        case nameof(Transfer):
          this.HandleNetworkTransfer(args);
          break;
        case nameof(OnItemTransferRequest):
          this.OnItemTransferRequest(args);
          break;
      }
    }

    protected override void HandleNetworkTransfer(object[] args)
    {
      var dict = args[0] as Dictionary<string, object>;
      int playerNetworkId = (int)dict[ToolTransferArgs.PLAYER_NETWORK_ID_KEY];
      if (dict.TryGetValue(
          ToolTransferArgs.ITEM_ID_KEY, out object itemId) &&
        itemId != null) {
        if (this.NetworkSynchronizer.TryFindComponentFromNetworkId(
            networId: (int)itemId,
            out Item foundItem)) {
          foundItem.gameObject.transform.SetParent(this.transform);
          foundItem.gameObject.transform.position = this.itemPoint.position;
          foundItem.gameObject.transform.rotation = this.itemPoint.rotation;
          Debug.Log($"ItemToGive: {foundItem}");
          this.tool.Transfer(new ToolTransferArgs {
              ItemToGive = foundItem,
              PlayerNetworkId = playerNetworkId
            });
        }
        #if UNITY_EDITOR
        else {
          Debug.LogError($"item not found for {args[0]}");
        }
        #endif
      }
      else {
        //FIXME: Return item to player
        GameObject playerObject = this.NetworkSynchronizer.GetPlayerObject();
        var result = this.tool.Transfer(new ToolTransferArgs {
          ItemToGive = null,
          PlayerNetworkId = playerNetworkId });
//        if (result.ReceivedItem != null && 
//          this.NetworkSynchronizer.TryGetGameObjectNetworkId(
//            result.ReceivedItem.gameObject, out int foundId) ) {
//          this.NetworkSynchronizer.SendRpcToGameObject(
//            gameObject: playerObject,
//            method: "PickUpObject",
//            args: new object[] { foundId });
//        }
//        #if UNITY_EDITOR
//        else {
//          Debug.LogError($"{nameof(HandleNetworkTransfer)}: Fail to return item to player");
//        }
//        #endif
      }
    }

    void OnItemTransferRequest(object[] args)
    {
      var dict = args[0] as Dictionary<string, object>;
      int playerNetworkId = (int)dict[ToolTransferArgs.PLAYER_NETWORK_ID_KEY];
      if (dict.TryGetValue(
          ToolTransferArgs.ITEM_ID_KEY, out object itemId) &&
        itemId != null) {
        if (this.NetworkSynchronizer.TryFindComponentFromNetworkId(
            networId: (int)itemId,
            out Item foundItem)) {
          this.Transfer(
            new ToolTransferArgs {
              ItemToGive = foundItem,
              PlayerNetworkId = playerNetworkId
            }); 
        }
        #if UNITY_EDITOR
        else {
          Debug.LogError($"{nameof(OnItemTransferRequest)}: Fail to find item {itemId}");
        }
        #endif
      }
      else {
        this.Transfer(new ToolTransferArgs {
          ItemToGive = null,
          PlayerNetworkId = playerNetworkId
          });
      }
    }

    public override ToolWorkResult Work(bool fromNetwork = false)
    {
      return (this.tool.Work());
    }

    protected override void Awake()
    {
      this.table = new DropOffTable(this.tableData);
      base.meshRenderer = this.model;
      base.Awake();
    }
  }
}
