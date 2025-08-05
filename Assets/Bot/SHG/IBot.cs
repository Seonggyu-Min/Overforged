using UnityEngine;
using UnityEngine.AI;

namespace SHG
{
  using ItemBox = SCR.BoxComponent;

  public interface IBot
  {

    const float ITEM_TRANSFER_DIST = 2.5f;
    const float ITEM_TRANSFER_DELAY = 1.0f;
    const float WORK_DIST = 1.5f;
    const float WORK_DELAY = 1f;

    public int NetworkId { get; }
    public Item HoldingItem { get; }
    public NavMeshAgent NavMeshAgent { get; }
    public Transform Transform { get; }
    public void GrabItem(Item item);
    public bool TryTransferItem(IInteractableTool tool);
    public ToolWorkResult Work(IInteractableTool tool);
    public void WaitForSeconds(float second);
    public bool IsHoldingTong { get; }
    public void PutDownTong();
    public void PickUpTong(Transform tong);
    public bool TryFindTool(SmithingTool.ToolType toolType, out SmithingToolComponent tool);
    public bool TryFindBox(RawMaterial rawMaterial, out ItemBox box);
    public Transform[] GetTongs();
    public T GetLeaf<T>(BtLeaf.Type leafType) where T: BtLeaf;
    public bool IsHoldingHotMaterial();

    public bool IsStopped => (!this.NavMeshAgent.pathPending &&
      (this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance || !this.NavMeshAgent.hasPath));
  }
}
