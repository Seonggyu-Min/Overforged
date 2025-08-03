using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using JJY;
using System;

public class NewProductConvey : SmithingTool
{

    [SerializeField] RecipeManager recipeManager;

    private ProductItem HoldingProductItem;
    public override bool IsFinished => false;

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;



    public NewProductConvey(SmithingToolData data) : base(data)
    {
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
        bool result = false;
        if (HoldingProductItem == null && args.ItemToGive != null)
        {
            if (args.ItemToGive is ProductItem item)
            {
                result = recipeManager.Check(item.Data as ProductItemData, item.Ore, item.Wood);
            }
        }
        Debug.Log(result);
        return result;
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
        this.BeforeInteract?.Invoke(this);
        if (args.ItemToGive != null)
        {
            if (!(args.ItemToGive is ProductItem productItem))
            {
                return (new ToolTransferResult());
            }
            return (this.ReturnWithEvent(
                this.ReceiveProductItem(productItem), args));
        }
        return (new ToolTransferResult());
    }

    protected ToolTransferResult ReceiveProductItem(ProductItem productItem)
    {
        this.HoldingProductItem = productItem;
        this.InteractionToTrigger = InteractionType.ReceivedItem;
        ToolTransferResult result = new ToolTransferResult
        {
            ReceivedItem = null,
            IsDone = false
        };
        return (result);
    }

    public override bool CanWork()
    {
        return false;
    }

    public override ToolWorkResult Work()
    {
        return new ToolWorkResult();
    }
}
