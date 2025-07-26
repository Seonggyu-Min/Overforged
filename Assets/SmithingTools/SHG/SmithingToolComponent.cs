using UnityEngine;

namespace SHG
{
    public abstract class SmithingToolComponent : MonoBehaviour, IInteractableTool, INetSynchronizable
  {
    protected SmithingTool tool;

    public int PlayerNetworkId { get; set; }

    public bool IsOwner { get; protected set; }

    public int SceneId { get; set; }

    public virtual bool CanTransferItem(ToolTransferArgs args)
    {
      bool canTransfer = this.tool.CanTransferItem(args);
      Debug.Log($"{nameof(CanTransferItem)}: {canTransfer}");
      return (canTransfer);
    }

    public virtual ToolTransferResult Transfer(ToolTransferArgs args)
    {
      var result = this.tool.Transfer(args);
      Debug.Log($"{nameof(Transfer)} result: {result}");
      if (args.ItemToGive != null) {
        args.ItemToGive.transform.SetParent(this.transform);
        args.ItemToGive.transform.localPosition = Vector3.up;
      }
      return (result);
    }

    public virtual bool CanWork() 
    {
      bool canwork = this.tool.CanWork();
      Debug.Log($"{nameof(canwork)}: {canwork}");
      return (canwork);
    }

    public virtual ToolWorkResult Work()
    {
      var result = this.tool.Work();
      Debug.Log($"{nameof(Work)} result: {result}");
      return (result);
    }

    public abstract void OnRpc(string method, float latencyInSeconds, object[] args = null);

    public abstract void SendRpc(string method, object[] args);

  }
}
