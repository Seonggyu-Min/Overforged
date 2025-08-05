using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using ItemBox = SCR.BoxComponent;

  [Serializable]
  public class BtBringMaterialLeaf: BtLeaf
  {
    [SerializeField]
    ItemBox box;
    IBot bot;
    [SerializeField]
    bool needWaiting;

    public BtBringMaterialLeaf(
      ItemBox box,
      IBot bot,
      BtNode parent = null) 
      : base(parent)
    {
      this.bot = bot;
      this.box = box; 
    }

    public BtBringMaterialLeaf Init(ItemBox box)
    {
      this.box = box;
      return (this);
    }

    public override NodeState Evaluate()
    {
      if (bot.HoldingItem != null) {
        return (this.ReturnState(NodeState.Success));
      }
      if (this.bot.IsStopped) {
        float dist = Vector3.Distance(
          this.box.transform.position,
          this.bot.Transform.position
          );
        if (dist <= IBot.ITEM_TRANSFER_DIST) {
          if (this.needWaiting) {
            this.bot.WaitForSeconds(IBot.ITEM_TRANSFER_DELAY);
            this.needWaiting = false;
            return (this.ReturnState(NodeState.Running));
          }
          Item item = this.box.CreateItem().GetComponent<Item>();
          if (item != null) {
            this.bot.GrabItem(item);
            return (this.ReturnState(NodeState.Success));
          }
          #if UNITY_EDITOR
          else {
            Debug.LogError($"{this}: Fail to Get item from {nameof(ItemBox)}");
          }
          #endif
        }
        else {
          this.needWaiting = true;
          this.bot.NavMeshAgent.SetDestination(
            this.box.transform.position);
          return (this.ReturnState(NodeState.Running));
        }
      }
      return (this.State);
    }
  }
}
