using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;

public class Convey : SmithingTool
{
    public override bool IsFinished => (this.RemainingInteractionCount < 1);

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;

    public Convey(SmithingToolData data): base(data)
    {
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
        return false;
    }

    public override bool CanWork()
    {
        return false;
    }

    public override ToolWorkResult Work()
    {
      this.InteractionToTrigger = InteractionType.Work;
      this.BeforeInteract?.Invoke(this);  
      ToolWorkResult result = new ToolWorkResult {};
      if (!this.IsFinished) {
        result = this.DecreseInteractionCount(this.InteractionTime);
      }
      if (this.IsFinished) {
        result = this.ChangeMaterial(this.InteractionTime);
      }
      return (this.ReturnWithEvent(result));
    }
}
