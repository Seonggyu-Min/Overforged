using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtConditionLeaf : BtLeaf
  {
    Func<bool> condition;
    NodeState falseState;

    public BtConditionLeaf(
      Func<bool> condition,
      NodeState falseState = NodeState.Failure,
      BtNode parent = null
      ): base(parent)
    {
      this.Init(condition, falseState);
    }

    public void Init(
      Func<bool> condition,
      NodeState falseSTate = NodeState.Failure)
    {
      this.condition = condition;      
      this.falseState = falseState;
    }

    public override NodeState Evaluate()
    {
      if (this.condition()) {
        return (this.ReturnState(NodeState.Success));
      }
      return (this.ReturnState(this.falseState));
    }
  }
}
