using System;
using System.Collections.Generic;

namespace SHG
{
  [Serializable]
  public class EnemyBotBt : BehaviourTree
  {
    public EnemyBotBt(
      IList<BtNode> children = null
      ) : base(children)
    {
    }
  }
}
