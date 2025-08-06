using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using ConveyCompontent = LocalProductConvey;

  public class BtSubmitProductLeaf : BtLeaf
  {
    ConveyCompontent submitPlace;
    IBot bot;

    public BtSubmitProductLeaf(
      IBot bot,
      BtNode parent = null): base(parent)
    {
      this.bot = bot;

    }

    public override NodeState Evaluate()
    {
      if (this.bot.HoldingItem == null) {
        return (this.ReturnState(NodeState.Failure));
      }
      if (this.submitPlace == null &&
        !this.bot.TryGetSubmitPlace(out this.submitPlace)) {
        return (this.ReturnState(NodeState.Failure));
      }
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.bot.Transform.position,
          this.submitPlace.transform.position);

        if (dist <= IBot.ITEM_TRANSFER_DIST) {
          if (this.bot.TryTransferItem(this.submitPlace)) {
            return (this.ReturnState(NodeState.Success));
          }
          return (this.ReturnState(NodeState.Failure));
        }
        else {
          this.bot.NavMeshAgent.SetDestination(
            this.submitPlace.transform.position);
          this.bot.NavMeshAgent.isStopped = false;
          return (this.ReturnState(NodeState.Running));
        }
      }
      return (this.ReturnState(NodeState.Running));
    }

    public override void Reset() 
    {
      this.submitPlace = null;
    }
  }
}
