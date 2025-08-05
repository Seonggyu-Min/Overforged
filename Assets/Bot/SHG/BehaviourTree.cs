using System;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  [Serializable]
  public abstract class BehaviourTree: BtNode
  {
    [SerializeField] [ReadOnly]
    string currentNodeName; 

    public BehaviourTree(
      IList<BtNode> children = null
      ): base(null, children)
    {
    }

    public override NodeState Evaluate()
    {
      while (this.currentChildIndex < this.children.Count) {
        this.currentNodeName = this.GetLastNode().ToString();
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
