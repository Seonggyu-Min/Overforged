using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtMoveLeaf : BtLeaf
  {
    Vector3 dest;
    IBot bot;
    float distThreshold;

    public BtMoveLeaf(
      Vector3 target,
      IBot bot,
      float dist,
      BtNode parent = null
      ): base(parent)
    {
      this.bot = bot;
      this.Init(target, dist);
    }

    public BtMoveLeaf Init(Vector3 dest, Nullable<float> distThreshold = null)
    {
      this.dest = dest;
      if (distThreshold != null) {
        this.distThreshold = distThreshold.Value;
      }
      return (this);
    }

    public override NodeState Evaluate()
    {
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.bot.Transform.position,
          this.dest
          );
        if (dist < this.distThreshold) {
          return (this.ReturnState(NodeState.Success));
        }
        else {
          this.bot.NavMeshAgent.SetDestination(this.dest);
          this.bot.NavMeshAgent.isStopped = false;
        }
      }
      return (this.ReturnState(NodeState.Running));
    }
  }
}
