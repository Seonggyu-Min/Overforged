using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SHG
{
  public abstract class SmithingToolComponent : MonoBehaviour, IInteractableTool, INetworkSynchronizable, IHighlightable
  {
    [Inject]
    protected INetworkSynchronizer<SmithingToolComponent> NetworkSynchronizer { get; private set; }

    protected abstract SmithingTool tool { get; }
    public virtual Item HoldingItem => this.tool.HoldingItem;
    public bool IsOwner
    {
      get => this.isOwner;
      set => this.isOwner = value;
    }
    public int PlayerNetworkId
    {
      get => this.playerId;
      set => this.playerId = value;
    }
    public int SceneId => this.id;

    public bool IsHighlighted => this.highlighter.IsHighlighted;

    public Color HighlightColor
    {
      get => this.highlighter.HighlightColor;
      set => this.highlighter.HighlightColor = value;
    }
    protected MeshRenderer meshRenderer;
    protected GameObjectHighlighter highlighter;
    protected abstract Transform materialPoint { get; }

    [SerializeField]
    int id;
    [SerializeField]
    int playerId;
    [SerializeField]
    bool isOwner;
    public Action<SmithingToolComponent, ToolTransferArgs, ToolTransferResult> OnTransfered;
    public Action<SmithingToolComponent, ToolWorkResult> OnWorked;
    protected abstract ISmithingToolEffecter effecter { get; }

    protected virtual void Awake()
    {
      this.highlighter = new GameObjectHighlighter(
        new Material[] { this.meshRenderer.material });
      if (this.meshRenderer != null)
      {
        this.meshRenderer.material = this.highlighter.HighlightedMaterials[0];
      }
    }

    protected virtual void Start()
    {
      this.NetworkSynchronizer?.RegisterSynchronizable(this);
    }

    protected virtual void Update()
    {
      this.highlighter.OnUpdate(Time.deltaTime);
      this.tool.OnUpdate(Time.deltaTime);
      this.effecter.OnUpdate(Time.deltaTime);
    }

    public virtual bool CanTransferItem(ToolTransferArgs args)
    {
#if LOCAL_TEST
      return (this.tool.CanTransferItem(args));
#else
      return (this.IsOwner && this.tool.CanTransferItem(args));
#endif
    }

    public virtual ToolTransferResult Transfer(ToolTransferArgs args)
    {
      var result = this.tool.Transfer(args);
      Debug.Log($"{nameof(Transfer)} result: {result}");
      if (args.ItemToGive != null)
      {
        args.ItemToGive.transform.SetParent(this.transform);
        args.ItemToGive.transform.position = this.materialPoint.position;
        args.ItemToGive.transform.up = this.materialPoint.up;
      }
      if (this.PlayerNetworkId != args.PlayerNetworkId)
      {
#if UNITY_EDITOR && !LOCAL_TEST
        throw (new ApplicationException($"{this} component is not owned by player"));
#endif
      }
      this.OnTransfered?.Invoke(this, args, result);
      return (result);
    }

    public virtual bool CanWork()
    {
#if LOCAL_TEST
      return (this.tool.CanWork());
#else
      return (this.IsOwner && this.tool.CanWork());
#endif
    }

    public virtual ToolWorkResult Work()
    {
      var result = this.tool.Work();
      Debug.Log($"{nameof(Work)} result: {result}");
      this.OnWorked?.Invoke(this, result);
      return (result);
    }

    public virtual void OnRpc(string method, float latencyInSeconds, object[] args = null)
    {
      switch (method)
      {
        case nameof(Transfer):
          this.HandleNetworkTransfer(args);
          break;
        case nameof(Work):
          this.HandleNetworkWork(args);
          break;
      }
    }

    protected virtual void HandleNetworkWork(object[] args)
    {
      // TODO: handle work result
      var dict = args[0] as Dictionary<string, object>;
      this.Work();
      // TODO: handle work trigger
      this.tool.OnInteractionTriggered?.Invoke(this.tool.InteractionToTrigger);
    }

    protected virtual void HandleNetworkTransfer(object[] args)
    {
      var dict = args[0] as Dictionary<string, object>;
      int playerNetworkId = (int)dict[ToolTransferArgs.PLAYER_NETWORK_ID_KEY];
      if (dict.TryGetValue(
          ToolTransferArgs.ITEM_ID_KEY, out object itemId) &&
        itemId != null)
      {
        if (this.NetworkSynchronizer != null &&
          this.NetworkSynchronizer.TryFindComponentFromNetworkId(
            networId: (int)itemId,
            out MaterialItem foundItem
            ))
        {
          this.Transfer(new ToolTransferArgs
          {
            ItemToGive = foundItem,
            PlayerNetworkId = playerNetworkId
          });
        }
#if UNITY_EDITOR
        else
        {
          Debug.LogError($"item not found for {args[0]}");
        }
#endif
      }
      else
      {
        //FIXME: Return item to player
        this.tool.HoldingItem.transform.SetParent(null);
        this.Transfer(new ToolTransferArgs
        {
          ItemToGive = null,
          PlayerNetworkId = playerNetworkId
        });
      }
    }

    public void HighlightInstantly(Color color)
    {
      this.highlighter.HighlightInstantly(color);
    }

    public void HighlightForSeconds(float seconds, Color color)
    {
      this.highlighter.HighlightForSeconds(seconds, color);
    }
  }
}
