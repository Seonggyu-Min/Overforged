using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class ItemStoreTable : IInteractableTool
  {
    public Item HoldingItem { get; private set; }

    public bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        return (this.HoldingItem == null);
      }
      else {
        return (this.HoldingItem != null);
      }
    }

    public bool CanWork()
    {
      return (false);
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (this.HoldingItem != null) {
        ToolTransferResult result = new ToolTransferResult {
            ReceivedItem = this.HoldingItem,
            IsDone = true
          };
        this.HoldingItem = null;
        return (result);
      }
      else {
        this.HoldingItem = args.ItemToGive;
        return (new ToolTransferResult {
            ReceivedItem = null,
            IsDone = true
          });
      }
    }

    public ToolWorkResult Work()
    {
      #if UNITY_EDITOR 
      throw (new ApplicationException($"{nameof(ItemStoreTable)} is unable to {nameof(Work)}"));
      #else
      return (new ToolWorkResult{});
      #endif
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}
