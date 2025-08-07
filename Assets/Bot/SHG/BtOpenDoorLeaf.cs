using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtOpenDoorLeaf : BtLeaf
  {
    IBot bot;
    DoorController door;
    public BtOpenDoorLeaf(
      IBot bot,
      BtNode parent = null
      ): base(parent)
    {
      this.bot = bot;
      this.door = BotContext.Instance.Door;
    }

    public override NodeState Evaluate()
    {
      if (!this.door.IsClosed) {
        return (this.ReturnState(NodeState.Success));
      }
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.bot.Transform.position,
          this.door.transform.position);
        if (dist <= IBot.WORK_DIST) {
          this.bot.Work(this.door);
          if (!this.door.IsClosed) {
            return (this.ReturnState(NodeState.Success));
          }
          else {
            return (this.ReturnState(NodeState.Failure));
          }
        }
        this.bot.NavMeshAgent.SetDestination(
          this.door.transform.position);
        this.bot.NavMeshAgent.isStopped = false;
      }
      return (this.ReturnState(NodeState.Running));
    }
  }
}
