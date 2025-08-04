using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using JJY;
using System;
using Photon.Pun;
using Zenject;
using System.Xml.XPath;

public class NewProductConvey : SmithingTool
{

    RecipeManager recipeManager;

    [Inject] MIN.IScoreManager _scoreManager;


    public ProductItem HoldingProductItem;
    public override bool IsFinished => false;

    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;



    public NewProductConvey(SmithingToolData data, RecipeManager manager) : base(data)
    {
        recipeManager = manager;
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
        bool result = false;
        if (HoldingProductItem == null && args.ItemToGive != null)
        {
            if (args.ItemToGive is ProductItem item)
            {
                result = true;
                //result = recipeManager.Check(item.Data as ProductItemData, item.Ore, item.Wood);
            }
        }
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
