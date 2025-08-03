using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public class ConveyorBeltHopper : SmithingToolComponent
  {
    [SerializeField] [Required]
    Transform boxPoint;
    [SerializeField] [Required]
    MeshRenderer model;
    [SerializeField] [Required]
    ConveyorBeltController beltController;
    [SerializeField] [Range(0f, 1f)]
    float boxOffset;
    ConveyorBelt conveyorBelt => this.beltController.ConveyorBelt;
    protected override SmithingTool tool => this.beltController.ConveyorBelt;
    protected override Transform materialPoint => this.boxPoint;
    protected override ISmithingToolEffecter effecter => this.beltController.Effecter;
    HashSet<ConveyorBeltBox> nearBoxes;

    protected override void Awake()
    {
      base.meshRenderer = this.model;
      base.Awake();
      this.nearBoxes = new ();
    }
    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (this.nearBoxes.Count == 0 &&
          this.tool.CanTransferItem(args));
      }
      else {
        return (this.nearBoxes.Count > 0);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.gameObject.SetActive(false); 
        var result = this.tool.Transfer(args);
        if (this.conveyorBelt.TryGetProcessingBox(
          out ConveyorBeltBox box)) {
          box.transform.position = this.boxPoint.position;
          box.transform.rotation = this.boxPoint.rotation;
          box.SetSpeed(this.beltController.Speed);
          box.SetOffset(this.boxOffset);
          box.StartMoveAlong(this.beltController.SplineContainer);
        }
        return (result);
      }
      else {
        if (this.TryGetNearestBox(out ConveyorBeltBox nearestBox) &&
          this.conveyorBelt.TrySetProcessingBox(nearestBox)) {
          var result = this.conveyorBelt.Transfer(args);
          if (result.ReceivedItem != null) {
            result.ReceivedItem.gameObject.SetActive(true);
          }
          this.nearBoxes.Remove(nearestBox);
          this.beltController.ReturnBox(nearestBox);          
          return (result);
        }
        #if UNITY_EDITOR
        Debug.LogError($"{nameof(ConveyorBelt)} {nameof(Transfer)} fail to find nearest box or set box");
        #endif
        return (new ToolTransferResult {});
      }
    }

    public override bool CanWork()
    {
      return (!this.beltController.IsTurningPower);
    }

    public override ToolWorkResult Work()
    {
      if (this.conveyorBelt.IsPowerOn) {
        this.beltController.TurnOff();
      }
      else {
        this.beltController.TurnOn();
      }
      return (this.tool.Work());
    }

    void OnTriggerEnter(Collider collider)
    {
      var box = collider.GetComponent<ConveyorBeltBox>();
      if (box != null) {
        this.nearBoxes.Add(box);
      }
    }

    void OnTriggerExit(Collider collider)
    {
      var box = collider.GetComponent<ConveyorBeltBox>();
      if (box != null) {
        this.nearBoxes.Remove(box);
      }
    }

    bool TryGetNearestBox(out ConveyorBeltBox nearest)
    {
      nearest = null;
      if (this.nearBoxes.Count > 0) {
        float dist = float.MaxValue;
        foreach (var box in this.nearBoxes) {
          float boxDist = Vector3.Distance(
            box.transform.position, this.transform.position); 
          if (boxDist < dist) {
            dist = boxDist;
            nearest = box;
          }
          return (true);
        }
      }
      return (false);
    }
  }
}
