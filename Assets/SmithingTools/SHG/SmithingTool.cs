using System;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

  [Serializable]
  public abstract class SmithingTool : IInteractable
  {
    public abstract bool IsFinished { get; }
    [ShowInInspector]
    public MaterialItem HoldingItem { get; protected set; }
    public MaterialType[] AllowedMaterials => this.Data.AllowedMaterials;
    public Action<SmithingTool, PlayerInteractArgs> BeforeInteract;
    public Action<SmithingTool, ToolInteractArgs> AfterInteract;
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

    public abstract bool IsInteractable(PlayerInteractArgs args);
    public abstract ToolInteractArgs Interact(PlayerInteractArgs args);

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

    protected ToolInteractArgs DecreseInteractionCount(float durationToStay)
    {
      this.RemainingInteractionCount -= 1;
      return (new ToolInteractArgs {
        ReceivedItem = null,
        DurationToPlayerStay = durationToStay,
        IsMaterialItemTaken = false,
        OnTrigger = this.OnTriggered
      });
    }

    protected ToolInteractArgs ReceiveMaterialItem(MaterialItem materialItem, float durationToStay = 0)
    {
      this.HoldingItem = materialItem;
      ToolInteractArgs result = new ToolInteractArgs {
        ReceivedItem = null,
        DurationToPlayerStay = durationToStay,
        IsMaterialItemTaken = true,
        OnTrigger = this.OnTriggered
      };
      return (result);
    }

    protected ToolInteractArgs ReturnItem(float durationToStay = 0)
    {
      var item = this.ItemToReturn;
      this.ResetInteraction();
      return (new ToolInteractArgs {
        ReceivedItem = item,
        DurationToPlayerStay = durationToStay,
        IsMaterialItemTaken = false,
        OnTrigger = this.OnTriggered
      });
    }

    protected ToolInteractArgs ReturnWithEvent(in ToolInteractArgs result)
    {
      this.AfterInteract?.Invoke(this, result);
      return (result);
    }

    protected virtual void ResetInteraction()
    {
      this.RemainingTime = this.DefaultRequiredTime;
      this.RemainingInteractionCount = this.DefaultRequiredInteractCount;
      this.HoldingItem = null;
    }
    protected abstract void OnTriggered(IInteractable interactable);
  }
}

