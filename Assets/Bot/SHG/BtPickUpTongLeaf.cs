using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtPickUpTongLeaf : BtLeaf
  {
    Transform[] tongs;
    IBot bot;
    Transform targetTong;

    public BtPickUpTongLeaf(
      IBot bot,
      BtNode parent = null): base(parent)
    {
      this.bot = bot;
      this.Init();
    }

    public BtPickUpTongLeaf Init()
    {
      this.tongs = this.bot.GetTongs();
      if (this.tongs.Length != 0) {
        this.TryGetClosestTong(out this.targetTong);
      }
      return (this);
    }

    bool TryGetClosestTong(out Transform tong)
    {
      tong = null;
      if (this.tongs.Length == 1) {
        if (this.IsTongPickUpAble(this.tongs[0])) {
          tong = (this.tongs[0]);
          return (true);
        }
      }
      else if (this.tongs.Length > 1) {
        float dist = float.MaxValue;
        foreach (var curTong in this.tongs) {
          float curDist = Vector3.Distance(
            this.bot.Transform.position,
            curTong.position
            ); 
          if (curDist < dist && this.IsTongPickUpAble(curTong)) {
            tong = curTong;
            dist = curDist;
          }
        }
        return (tong != null);
      }
      return (false);
    }

    bool IsTongPickUpAble(in Transform tong)
    {
      return (tong.parent == null ||
        tong.parent.gameObject.tag != "Player");
    }

    public override NodeState Evaluate() 
    {
      if (this.bot.IsHoldingTong) {
        return (this.ReturnState(NodeState.Success));
      }
      if (this.targetTong == null ||
        (!this.IsTongPickUpAble(this.targetTong))) {
        var pickup = this.TryGetClosestTong(out this.targetTong); 
      } 
      if (this.targetTong == null) {
        return (this.ReturnState(NodeState.Failure));
      }
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.targetTong.position,
          this.bot.Transform.position
          );
        if (dist < IBot.ITEM_TRANSFER_DIST) {
          this.bot.PickUpTong(this.targetTong);
          this.Reset();
          return (this.ReturnState(NodeState.Success));
        }
        else {
          this.bot.NavMeshAgent.SetDestination(this.targetTong.position);
          this.bot.NavMeshAgent.isStopped = false;
          return (this.ReturnState(NodeState.Running));
        }
      }
      else {
        return (this.ReturnState(NodeState.Running));
      }
    }

    public override void Reset()
    {
      this.targetTong = null;
    }
  }
}
