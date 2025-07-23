using System;

namespace SHG
{
  using Item = TestItem;
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

  public abstract class SmithingTool: IInteractable 
  {
    public virtual bool IsFinished { get; }
    public MaterialItem HoldingItem { get; protected set; }
    public MaterialType[] AllowedMaterials => this.Data.AllowdMaterials;
    public Action<SmithingTool, PlayerInteractArgs> BeforeInteract;
    public Action<SmithingTool, ToolInteractArgs> AfterInteract;

    protected SmithingToolData Data;
    protected abstract bool isPlayerMovable { get; }
    protected abstract bool isRemamingTimeElapse { get; }
    protected virtual Item ItemToReturn => this.HoldingItem;
    protected bool isInteracting;
    protected float RemainingTime { get; set; }
    protected int RemainingInteractionCount { get; set; }
    protected float DefaultRequiredTime => this.Data.TimeRequiredInSeconds;
    protected int DefaultRequiredInteractCount => this.Data.RequiredInteractCount;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
      this.isInteracting = false;
    }

    protected virtual void OnUpdate(float deltaTime)
    {
      if (this.isRemamingTimeElapse && !this.isInteracting) {
        return ;
      }  
      this.RemainingTime -= deltaTime;
    }

    public abstract bool IsInteractable(PlayerInteractArgs args);
    public abstract ToolInteractArgs Interact(PlayerInteractArgs args);
  }
}

