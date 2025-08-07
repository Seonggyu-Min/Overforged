using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public abstract class BtLeaf : BtNode
  {
    public enum Type
    {
      Idle,
      MoveLeaf,
      GetMaterial,
      GiveItem,
      GetItem,
      Work,
      PickUpTong,
      PutDownTong,
      RepeatWork,
      OpenDoor
    }

    public BtLeaf(BtNode parent = null): base(parent, null)
    {

    }

    public override NodeState Evaluate() => throw new NotImplementedException();

    public override void Reset()
    {}

    public override BtNode GetLastNode()
    {
      return (this);
    }
  }
}
