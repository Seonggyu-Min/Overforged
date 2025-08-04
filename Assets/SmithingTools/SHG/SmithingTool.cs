using System;

namespace SHG
{

  [Serializable]
  public abstract class SmithingTool : IInteractableTool
  {
    public enum InteractionType 
    {
      ReceivedItem,
      ReturnItem,
      Work
    }

    public enum ToolType
    {
      None,
      Furnace,
      Anvil,
      QuenchingTool,
      WoodTable,
      CraftTable,
    }

    public abstract bool IsFinished { get; }
    public MaterialItem HoldingMaterial { get; protected set; }
    public MaterialVariation[] AllowedMaterials => this.Data.AllowedMaterials;
    public Action<SmithingTool> BeforeInteract;
    public Action<SmithingTool> AfterInteract;
    public float RemainingTime { get; protected set; }
    public int RemainingInteractionCount { get; protected set; }
    public float Progress => this.CalcProgress();
    public InteractionType InteractionToTrigger { get; protected set; }
    public Action<InteractionType> OnInteractionTriggered;
    public Action<ItemData> OnMaterialChanged;

    protected SmithingToolData Data;
    protected abstract bool isPlayerMovable { get; }
    protected abstract bool isRemamingTimeElapse { get; }
    protected virtual Item ItemToReturn => this.HoldingMaterial;
    protected float DefaultRequiredTime => this.Data.TimeRequiredInSeconds;
    protected float InteractionTime => this.Data.InteractionTime;
    protected int DefaultRequiredInteractCount => this.Data.RequiredInteractCount;

    protected SmithingTool(SmithingToolData data)
    {
      this.Data = data;
      this.RemainingInteractionCount = data.RequiredInteractCount;
      this.RemainingTime = data.TimeRequiredInSeconds;
    }

    public virtual void OnUpdate(float deltaTime)
    {
      if (!this.isRemamingTimeElapse ||
      this.HoldingMaterial == null || this.IsFinished) {
        return;
      }
      this.RemainingTime -= deltaTime;
      if (this.RemainingTime < 0) {
        this.RemainingInteractionCount -= 1;
        this.RemainingTime = this.DefaultRequiredTime;
      }
    }

    public abstract bool CanTransferItem(ToolTransferArgs args);
    public virtual ToolTransferResult Transfer(ToolTransferArgs args) 
    {
      this.BeforeInteract?.Invoke(this);
      if (args.ItemToGive != null) {
        if (!(args.ItemToGive is MaterialItem materialItem)) {
          #if UNITY_EDITOR
          throw new ArgumentException($"{nameof(args.ItemToGive)} is not {nameof(MaterialItem)}");
          #else
          return (new ToolTransferResult());
          #endif
        }
        return (this.ReturnWithEvent(
            this.ReceiveMaterialItem(materialItem)));
      } 
      return (this.ReturnWithEvent(this.ReturnItem()));    
    }

    public abstract bool CanWork();
    public abstract ToolWorkResult Work();

    protected float CalcProgress()
    {
      var countProgress = ((float)this.DefaultRequiredInteractCount -
        (float)this.RemainingInteractionCount) /
        (float)this.DefaultRequiredInteractCount;
      var timeProgress = this.DefaultRequiredTime == 0 ? 0: 
        (this.DefaultRequiredTime - this.RemainingTime) /
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
      this.HoldingMaterial = materialItem;
      this.InteractionToTrigger = InteractionType.ReceivedItem;
      ToolTransferResult result = new ToolTransferResult {
        ReceivedItem = null,
        IsDone = false
      };
      return (result);
    }

    protected ToolTransferResult ReturnItem()
    {
      this.InteractionToTrigger = InteractionType.ReturnItem;
      var item = this.ItemToReturn;
      this.HoldingMaterial = null;
      this.ResetInteraction();
      return (new ToolTransferResult { 
        ReceivedItem = item,
        IsDone = true
        });
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
    }

    protected ToolWorkResult ChangeMaterial(float durationToStay)
    {
      this.HoldingMaterial.ChangeToNext();
      this.OnMaterialChanged?.Invoke(this.HoldingMaterial.Data);
      this.InteractionToTrigger = InteractionType.Work;
      this.ResetInteraction();
      return (new ToolWorkResult {
        Trigger = this.OnTriggered,
        DurationToStay = durationToStay
      });
    }

    protected virtual void OnTriggered()
    {
      this.OnInteractionTriggered?.Invoke(this.InteractionToTrigger);
    }
  }
}

