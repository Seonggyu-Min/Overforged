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

    protected override void Awake()
    {
      base.meshRenderer = this.model;
      base.Awake();
    }
    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (this.tool.CanTransferItem(args));
      }
      else {
        return (false);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.gameObject.SetActive(false); 
      }
      var result = this.tool.Transfer(args);
      if (this.conveyorBelt.TryGetProcessingBox(out ConveyorBeltBox box)) {
        box.transform.position = this.boxPoint.position;
        box.transform.rotation = this.boxPoint.rotation;
        box.SetSpeed(this.beltController.Speed);
        box.SetOffset(this.boxOffset);
        box.StartMoveAlong(this.beltController.SplineContainer);
      }
      return (result);
    }

    public override bool CanWork()
    {
      return (this.tool.CanWork());
    }

    public override ToolWorkResult Work()
    {
      return base.Work();
    }
  }
}
