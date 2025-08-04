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
      if (this.condition()) {
        return (this.ReturnState(this.trueNode.Evaluate()));
      }  
      return (this.ReturnState(this.falseNode.Evaluate()));
    }
  }
}
