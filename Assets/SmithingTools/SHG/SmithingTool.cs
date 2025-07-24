using System;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

  [Serializable]
  public abstract class SmithingTool: IInteractable 
  {
    public virtual bool IsFinished { get; }
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
    protected bool isInteracting;
    protected float DefaultRequiredTime => this.Data.TimeRequiredInSeconds;
    protected int DefaultRequiredInteractCount => this.Data.RequiredInteractCount;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
      this.isInteracting = false;
      this.RemainingInteractionCount = data.RequiredInteractCount;
      this.RemainingTime = data.TimeRequiredInSeconds;
    }

    public virtual void OnUpdate(float deltaTime)
    {
      if (!this.isRemamingTimeElapse || !this.isInteracting) {
        return ;
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
      return (countProgress + (timeProgress / (float)this.DefaultRequiredInteractCount));
    }
  }
}

