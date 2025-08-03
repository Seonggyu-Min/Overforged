using System;
using System.Collections.Generic;

namespace SHG
{
  public class ConveyorBelt : SmithingTool
  {
    public bool IsPowerOn { get; private set; }
    public override bool IsFinished => true;
    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;
    public Dictionary<ConveyorBeltBox, Item> AllItemBox { get; private set; }
    ConveyorBeltBox processingBox;
    Func<ConveyorBeltBox> createBox;

    public ConveyorBelt(
      SmithingToolData data, 
      Func<ConveyorBeltBox> createBox) : base(data)
    {
      this.createBox = createBox;
      this.AllItemBox = new ();
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (!args.ItemToGive.IsHot);
      }
      else {
        return (this.AllItemBox.Count > 0);
      }
    }

    public bool TryGetProcessingBox(out ConveyorBeltBox box)
    {
      if (this.processingBox != null) {
        box = this.processingBox;
        this.processingBox = null;
        return (true); 
      }
      else {
        box = null;
        return (false);
      }
    }

    public bool TrySetProcessingBox(in ConveyorBeltBox box)
    {
      if (this.processingBox == null) {
        this.processingBox = box;
        return (true);
      }
      return (false);
    }

    bool TryGetBoxItem(ConveyorBeltBox box, out Item item)
    {
      if (this.AllItemBox.TryGetValue(box, out item)) {
        this.AllItemBox.Remove(box);
        return (true);
      }
      else {
        item = null;
        return (false);
      }
    }

    public override bool CanWork()
    {
      return (true);
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      this.BeforeInteract?.Invoke(this);
      if (args.ItemToGive != null) {
      #if UNITY_EDITOR
      if (this.processingBox != null) {
        throw (new ApplicationException($"{nameof(ConveyorBelt)} {nameof(Transfer)}: {nameof(processingBox)} is not null"));
      }
      #endif
      this.InteractionToTrigger = InteractionType.ReceivedItem;
      this.processingBox = this.createBox();
      this.AllItemBox.Add(this.processingBox, args.ItemToGive);
      return (this.ReturnWithEvent(
          new ToolTransferResult {
          ReceivedItem = null,
          IsDone = true}));
      } 
      else {
        this.InteractionToTrigger = InteractionType.ReturnItem;
        var result = new ToolTransferResult { IsDone = true };
        #if UNITY_EDITOR
        if (this.processingBox == null) {
          throw (new ApplicationException($"{nameof(ConveyorBelt)} {nameof(Transfer)}: {nameof(processingBox)} is not null"));
        #endif
        }
        if (this.TryGetBoxItem(this.processingBox, out Item item)) {
          result.ReceivedItem = item;
          this.AllItemBox.Remove(this.processingBox);
          this.processingBox = null;
        }
        #if UNITY_EDITOR
        else {
          throw (new ApplicationException($"{nameof(ConveyorBelt)} {nameof(Transfer)}: fail to find item for {nameof(processingBox)}"));
        #endif
        }
        return (this.ReturnWithEvent(result));    
      }
    }

    public override ToolWorkResult Work()
    {
      this.InteractionToTrigger = InteractionType.Work;
      this.IsPowerOn = !this.IsPowerOn;
      return (this.ReturnWithEvent(
          new ToolWorkResult {
            DurationToStay = 0f,
            Trigger = this.OnTriggered 
          }));
    }
  }
}
