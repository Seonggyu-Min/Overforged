using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public class BtWorkLeaf : BtLeaf
  {
    IInteractableTool tool;
    Vector3 dest;
    IBot bot;
    
    public BtWorkLeaf(
      IInteractableTool tool,
      Transform transform,
      IBot bot,
      BtNode parent = null
      ): base(parent)
    {
      this.bot = bot;
      if (tool != null && transform  != null) {
        this.Init(tool, transform);
      }
    }

    public void Init(IInteractableTool tool, Transform transform)
    {
      this.tool = tool;
      bool hasFront = tool is FurnaceComponent;
      this.dest = transform.position +
        (hasFront ? transform.forward * 0.5f: Vector3.zero);
    }

    public override NodeState Evaluate()
    {
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.dest,
          this.bot.Transform.position
          );
        if (dist < IBot.WORK_DIST) {
          if (this.tool.CanWork()) {
            var result = this.bot.Work(this.tool);
            this.bot.WaitForSeconds(IBot.WORK_DELAY);
            return (this.ReturnState(NodeState.Success));
          }
          else {
            return (this.ReturnState(NodeState.Failure));
          }
        }
        else {
          this.bot.NavMeshAgent.SetDestination(this.dest);
          this.bot.NavMeshAgent.isStopped = false;
          return (this.ReturnState(NodeState.Running));
        }
      }
      return (this.ReturnState(NodeState.Running));
    }
  }
}
