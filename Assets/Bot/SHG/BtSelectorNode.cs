using System.Collections.Generic;

namespace SHG
{
  public class BtSelectorNode : BtNode
  {
    public BtSelectorNode(
      BtNode parent = null,
      IList<BtNode> children = null) 
      : base(parent, children)
    {
    }

    public override NodeState Evaluate()
    {
      foreach (BtNode child in this.children) {
        switch (child.Evaluate()) {
          case (NodeState.Success):
            this.State = NodeState.Success;
            return (this.State);
          case (NodeState.Failure):
            continue;
          case (NodeState.Running):
            this.State = NodeState.Running;
            return (this.State);
        } 
      }
      this.State = NodeState.Failure;
      return (this.State);
    }
  }
}
