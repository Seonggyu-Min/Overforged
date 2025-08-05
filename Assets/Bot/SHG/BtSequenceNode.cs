using System.Collections.Generic;

namespace SHG
{
  public class BtSequenceNode : BtNode
  {
    public BtSequenceNode(
      BtNode parent = null,
      IList<BtNode> children = null) 
      : base(parent, children)
    {
    }

    public override NodeState Evaluate()
    {
      if (this.currentChildIndex < this.children.Count) {
        NodeState state = this.children[this.currentChildIndex].Evaluate();
        switch (state) {
          case (NodeState.Failure):
            this.Reset();
            return (this.ReturnState(state));
          case (NodeState.Running):
            return (this.ReturnState(state));
          case (NodeState.Success):
            if (++this.currentChildIndex == this.children.Count) {
              return (this.ReturnState(NodeState.Success));
            }
            return (this.ReturnState(NodeState.Running));
        } 
      }
      this.Reset();
      return (this.ReturnState(NodeState.Success));
    }
  }
}
