using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public abstract class BtLeaf : BtNode
  {

    public BtLeaf(
      BtNode parent = null): base(parent, null)
    {

    }

    public override NodeState Evaluate() => throw new NotImplementedException();

    public override void Reset()
    {}
  }
}
