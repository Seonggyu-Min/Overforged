using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public class EnemyBotBt : BehaviourTree
  {
    public EnemyBotBt(
      IBot bot,
      IList<BtNode> children = null
      ) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
      NodeState result = base.Evaluate();
      if (result == NodeState.Success && this.children.Count > 0) {
        Debug.Log($"done");
      }
      return (result);
    }
  }
}
