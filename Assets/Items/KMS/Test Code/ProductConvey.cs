using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using JJY;
using SHG;
using UnityEngine;
using UnityEngine.Jobs;

public class ProductConvey : SmithingToolComponent
{
    [SerializeField] Transform ProductPoint;
    //씬에서 할당 필요
    [SerializeField] RecipeManager recipeManager;

    Convey convey;

    private Item HoldingItem;
    protected override SmithingTool tool => null;
    protected override ISmithingToolEffecter effecter => null;

    protected override Transform materialPoint => ProductPoint;




    void Awake()
    {
        //UI.enabled = false;

    }


    public override bool CanTransferItem(ToolTransferArgs args)
    {
        bool result = false;
        if (HoldingItem == null && args.ItemToGive != null && args.ItemToGive is Item temp)
        {
            if (temp is ProductItem item)
            {
                result = recipeManager.Check(item.Data as ProductItemData, item.Ore, item.Wood);
            }
        }
        return result;
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
        if (args.ItemToGive != null)
        {
            HoldingItem = args.ItemToGive;
            args.ItemToGive.transform.SetParent(this.transform);
            args.ItemToGive.transform.position = this.materialPoint.position;
            args.ItemToGive.transform.up = this.materialPoint.up;
            StartCoroutine(ItemRemoveRoutine());
        }
        return (new ToolTransferResult
        {
            ReceivedItem = null,
            IsDone = false
        });
    }

    public override bool CanWork()
    {
        return false;
    }

    public override ToolWorkResult Work()
    {
        return (new ToolWorkResult
        {
            Trigger = null,
            DurationToStay = 0.1f
        });
    }

    private IEnumerator ItemRemoveRoutine()
    {
        yield return new WaitForSeconds(3);
        HoldingItem = null;
        recipeManager.FulfillRecipe();

    }

}
