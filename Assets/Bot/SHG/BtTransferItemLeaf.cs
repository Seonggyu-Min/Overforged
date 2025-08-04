using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtTransferItemLeaf : BtLeaf
  {
    IInteractableTool tool;
    IBot bot;
    Vector3 dest;
    bool needWaiting;
    bool toGive;

    public BtTransferItemLeaf(
      bool toGive,
      IInteractableTool tool,
      Transform transform,
      IBot bot,
      BtNode parent = null
      ): base(parent)
    {
      this.bot = bot;
      this.toGive = toGive;
      if (tool != null && transform != null) {
        this.Init(tool, transform, toGive);
      }
    }

    public void Init(IInteractableTool tool, Transform transform, Nullable<bool> toGive = null)
    {
      this.tool = tool;
      bool hasFront = tool is FurnaceComponent;
      this.dest = transform.position +
        (hasFront ?  transform.forward * 0.5f : Vector3.zero);
      if (toGive != null) {
        this.toGive = toGive.Value;
      }
    }

    public override NodeState Evaluate()
    {
      if (this.toGive && this.bot.HoldingItem == null) {
        return (this.ReturnState(NodeState.Success));
      }
      else if (!this.toGive && this.bot.HoldingItem != null) {
        return (this.ReturnState(NodeState.Success));
      }
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.dest,
          this.bot.Transform.position
          );
        if (dist <= IBot.ITEM_TRANSFER_DIST) {
          if (this.needWaiting) {
            this.needWaiting = false;
            this.bot.WaitForSeconds(IBot.ITEM_TRANSFER_DELAY);
            return (this.ReturnState(NodeState.Running));
          }
          if (this.bot.TryTransferItem(this.tool)) {
            return (this.ReturnState(NodeState.Success));
          }    
          else {
            return (this.ReturnState(NodeState.Failure));
          }
        }
        else {
          this.needWaiting = true;
          this.bot.NavMeshAgent.SetDestination(this.dest);
          this.bot.NavMeshAgent.isStopped = false;
        }
      }
      return (this.ReturnState(NodeState.Running));
    }

    public override void Reset()
    {
      this.needWaiting = true;
    }
  }
}
