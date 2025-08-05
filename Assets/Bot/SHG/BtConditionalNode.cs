using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtConditionalNode : BtNode
  {

    Func<bool> condition;
    BtNode trueNode;
    BtNode falseNode;
    BtNode runningNode;

    public BtConditionalNode(
      Func<bool> condition,
      BtNode trueNode,
      BtNode falseNode,
      BtNode parent = null
      ): base(parent, null)
    {
      this.condition = condition;
      this.trueNode = trueNode;
      this.falseNode = falseNode;
      this.children.Add(trueNode); 
      this.children.Add(falseNode); 
    }

    public override NodeState Evaluate() 
    {
      if (this.runningNode != null) {
        return (this.ReturnState(this.runningNode.Evaluate()));
      }
      if (this.condition()) {
        var state = this.trueNode.Evaluate();
        if (state == NodeState.Running) {
          this.runningNode = this.trueNode;
        }
        return (this.ReturnState(state));
      }  
      if (this.falseNode != null) {
        var state = this.falseNode.Evaluate();
        if (state == NodeState.Running) {
          this.runningNode = this.falseNode;
        }
        return (this.ReturnState(state));
      }
      return (this.ReturnState(NodeState.Success));
    }
  }
}
