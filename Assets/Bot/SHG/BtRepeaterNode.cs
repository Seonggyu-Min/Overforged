using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtRepeaterNode : BtNode
  {
    Func<bool> condition;
    int count;
    BtNode target;
    int currentCount;
    BtNode runningNode;

    public BtRepeaterNode(
      BtNode target,
      Func<bool> condition = null,
      int count = -1,
      BtNode parent = null
      ): base(parent, new BtNode[] { target })
    {
      this.Init(target, condition, count);
    }

    public BtRepeaterNode Init(BtNode target, Func<bool> condition = null, int count = -1)
    {
      if (condition == null && count == -1) {
        throw (new ArgumentException($"{nameof(count)} must be greater than 0 or {nameof(condition)} must be not null"));
      }
      this.target = target;
      this.condition = condition;
      this.count = count;
      return (this);
    }

    public override NodeState Evaluate() 
    {
      NodeState state = this.runningNode != null ?
        this.runningNode.Evaluate() :this.target.Evaluate();
      switch (state) {
        case (NodeState.Failure):
          this.runningNode = null;
          return (this.ReturnState(NodeState.Failure));
        case (NodeState.Running):
          this.runningNode = target;
          return (this.ReturnState(NodeState.Running));
        default:
          this.runningNode = null;
          if (this.condition == null) {
            this.currentCount += 1;
            if (this.currentCount >= this.count) {
              return (this.ReturnState(NodeState.Success)); 
            }
            return (this.ReturnState(NodeState.Running));
          }
          else {
            if (this.condition()) {
              return (this.ReturnState(NodeState.Success));
            }
            return (this.ReturnState(NodeState.Running));
          }
      }
    }

    public override void Reset()
    {
      this.currentCount = 0;
      this.target.Reset();
    }
  }
}
