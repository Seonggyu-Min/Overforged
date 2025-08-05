using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtQuenchingNode : BtSequenceNode
  {
    IBot bot;
    QuenchingComponent tool;

    public BtQuenchingNode(
      IBot bot,
      BtNode parent = null) : base(parent, null)
    {
      this.bot = bot;
      this.Init();
    }

    public BtQuenchingNode Init()
    {
      if (this.tool == null &&
        this.bot.TryFindTool(SmithingTool.ToolType.QuenchingTool,
          out SmithingToolComponent found))
      {
        this.tool = found as QuenchingComponent;
      }
      if (this.tool == null)
      {
        return (this);
      }
      this.children.Clear();
      this.AddChild(
        this.bot.GetLeaf<BtPickUpTongLeaf>(BtLeaf.Type.PickUpTong));
      this.AddChild(new BtTransferItemLeaf(
          toGive: true,
          tool: this.tool,
          transform: this.tool.transform,
          bot: this.bot));
      this.AddChild(new BtConditionLeaf(
          condition: () => this.tool.IsFinished,
          falseState: NodeState.Running));
      this.AddChild(new BtTransferItemLeaf(
          toGive: false,
          tool: this.tool,
          transform: this.tool.transform,
          bot: this.bot));
      return (this);
    }
  }
}
