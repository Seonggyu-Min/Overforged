using System;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

  [Serializable]
  public abstract class SmithingTool : IInteractableTool
  {
    public abstract bool IsFinished { get; }
    [ShowInInspector]
    public MaterialItem HoldingItem { get; protected set; }
    public MaterialType[] AllowedMaterials => this.Data.AllowedMaterials;
    public Action<SmithingTool> BeforeInteract;
    public Action<SmithingTool> AfterInteract;
    [ShowInInspector]
    public float RemainingTime { get; protected set; }
    [ShowInInspector]
    public int RemainingInteractionCount { get; protected set; }
    public float Progress => this.CalcProgress();

    [SerializeField]
    protected SmithingToolData Data;
    protected abstract bool isPlayerMovable { get; }
    protected abstract bool isRemamingTimeElapse { get; }
    protected abstract Item ItemToReturn { get; }
    [SerializeField]
    protected float DefaultRequiredTime => this.Data.TimeRequiredInSeconds;
    protected int DefaultRequiredInteractCount => this.Data.RequiredInteractCount;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
      this.RemainingInteractionCount = data.RequiredInteractCount;
      this.RemainingTime = data.TimeRequiredInSeconds;
    }

    public virtual void OnUpdate(float deltaTime)
    {
      if (this.IsFinished) {
        return ;
      }
      if (!this.isRemamingTimeElapse ||
      this.HoldingItem == null || this.IsFinished) {
        return;
      }
      this.RemainingTime -= deltaTime;
      if (this.RemainingTime < 0) {
        this.RemainingInteractionCount -= 1;
      }
    }

    public abstract bool CanTransferItem(ToolTransferArgs args);
    public virtual ToolTransferResult Transfer(ToolTransferArgs args) 
    {
      this.BeforeInteract?.Invoke(this);
      if (args.ItemToGive != null) {
        return (this.ReturnWithEvent(
            this.ReceiveMaterialItem(args.ItemToGive)));
      }
      return (this.ReturnWithEvent(
          this.ReturnItem()));    
    }

    public abstract bool CanWork();
    public abstract ToolWorkResult Work();

    protected float CalcProgress()
    {
      var countProgress = ((float)this.DefaultRequiredInteractCount -
        (float)this.RemainingInteractionCount) /
        (float)this.DefaultRequiredInteractCount;
      var timeProgress = (this.DefaultRequiredTime - this.RemainingTime) /
        this.DefaultRequiredTime;
      var progress = (countProgress + (timeProgress / (float)this.DefaultRequiredInteractCount));
      return (Math.Min(progress, 1f));
    }

    protected ToolWorkResult DecreseInteractionCount(float durationToStay)
    {
      this.RemainingInteractionCount -= 1;
      return (new ToolWorkResult {
        Trigger = this.OnTriggered,
        DurationToStay = durationToStay
      });
    }

    protected ToolTransferResult ReceiveMaterialItem(MaterialItem materialItem)
    {
      this.HoldingItem = materialItem;

      ToolTransferResult result = new ToolTransferResult {
        ReceivedItem = null
      };
      return (result);
    }

    protected ToolTransferResult ReturnItem()
    {
      var item = this.ItemToReturn;
      this.ResetInteraction();
      return (new ToolTransferResult { ReceivedItem = item });
    }

    protected ToolTransferResult ReturnWithEvent(in ToolTransferResult result)
    {
      this.AfterInteract?.Invoke(this);
      return (result);
    }

    protected ToolWorkResult ReturnWithEvent(in ToolWorkResult result)
    {
      this.AfterInteract?.Invoke(this);
      return (result);
    }

    protected virtual void ResetInteraction()
    {
      this.RemainingTime = this.DefaultRequiredTime;
      this.RemainingInteractionCount = this.DefaultRequiredInteractCount;
      this.HoldingItem = null;
    }
    protected abstract void OnTriggered();
  }
}

