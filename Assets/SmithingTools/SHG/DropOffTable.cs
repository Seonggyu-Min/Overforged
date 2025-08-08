using System;

namespace SHG
{
  public class DropOffTable : SmithingTool
  {
    public DropOffTable(SmithingToolData data) : base(data)
    {
    }

    public override bool IsFinished => true;
    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;
    public Item HoldingItem { get; private set;}

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (this.HoldingItem == null);
      }
      else {
        return (this.HoldingItem != null);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args, bool fromNetwork = false)
    {
      if (this.HoldingItem != null) {
        Item item = this.HoldingItem;
        this.HoldingItem = null;
        return (new ToolTransferResult {
          ReceivedItem = item,
          IsDone = true
        });
      }
      else {
        this.HoldingItem = args.ItemToGive;
        return (new ToolTransferResult {
          ReceivedItem = null,
          IsDone = true
          });
      }
    }

    public override bool CanWork()
    {
      return (false);
    }

    public override ToolWorkResult Work(bool fromNetwork = false)
    {
      #if UNITY_EDITOR 
      throw (new ApplicationException($"{nameof(DropOffTable)} is unable to {nameof(Work)}"));
      #else
      return (new ToolWorkResult{});
      #endif
    }
  }
}
