using System;
using System.Collections;
using System.Collections.Generic;
using JJY;
using SHG;
using UnityEngine;

public class ProductConvey : MonoBehaviour
{
    //씬에서 할당 필요
    [SerializeField] RecipeManager recipeManager;

    //집게는 없으나 아이템은 가진 상황에서
    //손에 든 아이템에 ProductItem 컴포넌트가 있다면 (완성품이라면)
    //InteractionObj 태그로 출품대 오브젝트를 확인
    //Getconponent로 ProductConvey를 가져온 후
    //ProductItem을 넘겨주며 아래의 함수를 실행합니다.

    //아이템이 의뢰 품목에 있다면 true를 반환합니다.
    //그때 아이템 제거를 진행하시면 될 것 같습니다.
    public bool Check(ProductItem item)
    {
        bool result = recipeManager.FulfillRecipe(item.Data as ProductItemData, item.Ore, item.Wood);
        //점수 올리는 기능이 필요
        return result;
    }




    //void Awake()
    //{
    //    //UI.enabled = false;
//
    //}
//
//
    //public bool CanTransferItem(ToolTransferArgs args)
    //{
    //    return true;
    //}
//
    //public ToolTransferResult Transfer(ToolTransferArgs args)
    //{
    //    UI.enabled = false;
    //    return (new ToolTransferResult
    //    {
    //        ReceivedItem = null,
    //        IsDone = true
    //    });
    //}
//
    //public bool CanWork()
    //{
    //    return true;
    //}
//
    //public ToolWorkResult Work()
    //{
    //    if (UI.enabled)
    //    {
    //        UI.enabled = false;
    //        return (new ToolWorkResult
    //        {
    //            Trigger = null,
    //            DurationToStay = 0.1f
    //        });
//
    //    }
    //    else
    //    {
    //        UI.enabled = true;
    //        return (new ToolWorkResult
    //        {
    //            Trigger = null,
    //            DurationToStay = float.MaxValue
    //        });
    //    }
    //}

}
