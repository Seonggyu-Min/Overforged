using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public class DropOffTableComponent :SmithingToolComponent 
  {

    [SerializeField] [Required] 
    Transform itemPoint;
    [SerializeField] [Required]
    MeshRenderer model;
    [SerializeField] [Required]
    SmithingToolData tableData; 
    protected override SmithingTool tool => this.table;
    protected override Transform materialPoint => this.itemPoint;
    protected override ISmithingToolEffecter effecter => null;
    protected override bool isProgressUsed => false;
    DropOffTable table;

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      return (this.table.CanTransferItem(args));
    }

    public override bool CanWork()
    {
      return (this.table.CanWork());
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.gameObject.transform.SetParent(this.transform);
        args.ItemToGive.gameObject.transform.position = this.itemPoint.position;
        args.ItemToGive.gameObject.transform.rotation = this.itemPoint.rotation;
      }
      return (this.tool.Transfer(args));
    }

    public override ToolWorkResult Work()
    {
      return (this.tool.Work());
    }

    protected override void Awake()
    {
      this.table = new DropOffTable(this.tableData);
      base.meshRenderer = this.model;
      base.Awake();
    }

  }
}
