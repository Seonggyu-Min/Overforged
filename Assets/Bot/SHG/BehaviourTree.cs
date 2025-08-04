using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public abstract class BehaviourTree: BtNode
  {

    public BehaviourTree(
      IList<BtNode> children = null
      ): base(null, children)
    {
    }

    public override NodeState Evaluate()
    {
      while (this.currentChildIndex < this.children.Count) {
        NodeState state = this.children[this.currentChildIndex].Evaluate();
        if (state != NodeState.Success) {
          return (this.ReturnState(state));
        }
        this.currentChildIndex += 1;
      }
      return (this.ReturnState(NodeState.Success));
    }
  }
}
