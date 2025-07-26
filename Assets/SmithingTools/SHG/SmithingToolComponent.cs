using UnityEngine;
using Zenject;
using EditorAttributes;

namespace SHG
{
    public abstract class SmithingToolComponent : MonoBehaviour, IInteractableTool, INetSynchronizable
  {
    [Inject]
    public INetworkSynchronizer<SmithingToolComponent> NetworkSynchronizer { get; private set; }

    protected SmithingTool tool;
    public bool IsOwner {
      get => this.isOwner;
      set => this.isOwner = value;
    }
    public int PlayerNetworkId {
      get => this.playerId;
      set => this.playerId = value;
    }
    public int SceneId => this.id;
    [SerializeField]
    int id;
    [SerializeField]
    int playerId;
    [SerializeField, ReadOnly]
    bool isOwner;

    protected virtual void Start()
    {
      this.NetworkSynchronizer.RegisterSynchronizable(this);
    }

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
