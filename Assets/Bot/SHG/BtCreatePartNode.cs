using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using ItemBox = SCR.BoxComponent;

  public class BtCreatePartNode : BtSequenceNode
  {
    Part part;
    IBot bot;
    RawMaterial rawMaterial;
    SmithingToolComponent tool;
    SmithingTool.ToolType toolType;

    public BtCreatePartNode(
      Part part,
      IBot bot,
      bool takePart = true,
      BtNode parent = null
      ): base(parent, null)
    {
      this.bot = bot;
      this.Init(part, takePart);
    }
    
    public BtCreatePartNode Init(Part part, bool takePart = true)
    {
      this.children.Clear();
      this.part = part;
      if (this.part.TryGetPreviousPart(out Part prevPart)) {
        this.AddChild(new BtCreatePartNode(
            part: prevPart,
            bot: this.bot));
      }
      else {
        var rawMaterial = this.part.TryGetRawMaterial();
        if (rawMaterial != null) {
          this.rawMaterial = rawMaterial.Value;
        }
        else {
          return (this);
        }
        if (!this.bot.TryFindBox(this.rawMaterial, out ItemBox box)) {
          return (this);
        }
        this.AddChild(new BtBringMaterialLeaf(
            box: box,
            bot: this.bot));
        if (this.part.Equals(Part.String)) {
          return (this);
        }
      }
      var toolType = this.part.GetToolType(); 
      if (toolType == null) {
        return (this); 
      }
      this.toolType = toolType.Value;
      if (this.bot.TryFindTool(this.toolType, out this.tool) ) {
        this.AddChild(new BtTransferItemLeaf(
            toGive: true,
            tool: this.tool,
            transform: this.tool.transform,
            bot: this.bot
            ));
        int workCount = SmithingToolConstants.Instance.GetWorkCount(
          this.toolType);
        if (toolType == SmithingTool.ToolType.Furnace && 
          this.tool is FurnaceComponent furnace) {
          if (!furnace.IsIgnited) {
            this.AddChild(new BtWorkLeaf(
                tool: this.tool,
                transform: this.tool.transform,
                bot: this.bot));
          }
          this.AddChild(this.bot.GetLeaf<BtPickUpTongLeaf>(
              BtLeaf.Type.PickUpTong));
          this.AddChild(
              new BtMoveLeaf(
                target: furnace.transform.position + 
                furnace.transform.forward * 0.5f,
                bot: this.bot,
                dist: IBot.ITEM_TRANSFER_DIST)
            );
          this.AddChild(new BtConditionLeaf(
              condition: () => furnace.IsFinished,
              falseState: NodeState.Running));
        }
        else {
          var work = new BtWorkLeaf(
              tool: this.tool,
              transform: this.tool.transform,
              bot: this.bot);
          this.AddChild(new BtRepeaterNode(
              target: work,
              count: workCount));
        }
        if (this.toolType == SmithingTool.ToolType.Anvil) {
          this.AddChild(new BtPickUpTongLeaf(bot: this.bot));
        }
        if (takePart) {
          this.AddChild(
            new BtTransferItemLeaf(
              toGive: false,
              tool: this.tool,
              transform: this.tool.transform,
              bot: this.bot)
            );
        }
      }
      #if UNITY_EDITOR
      else {
        Debug.LogError($"{nameof(BtCreatePartNode)}: fail to find box");  
      }
      #endif
      return (this);
    }
  }
}
