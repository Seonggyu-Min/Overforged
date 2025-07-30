using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using JJY;
using Photon.Pun;
using Photon.Realtime;
using SHG;
using UnityEngine;
using Zenject;

public class ProductConvey : MonoBehaviour, IInteractableTool
{
    [SerializeField] Transform ProductPoint;
    //씬에서 할당 필요
    [SerializeField] RecipeManager recipeManager;

    [SerializeField] PhotonView photon;

    [Inject] MIN.IScoreManager _scoreManager;

    Convey convey;

    private Item HoldingItem;




    void Awake()
    {
        //UI.enabled = false;

    }


    public bool CanTransferItem(ToolTransferArgs args)
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

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
        if (args.ItemToGive != null)
        {

            int id = args.ItemToGive.GetComponent<PhotonView>().ViewID;
            photon.RPC("SetItemRPC", RpcTarget.All, args.PlayerNetworkId, id);
        }
        return (new ToolTransferResult
        {
            ReceivedItem = null,
            IsDone = false
        });
    }

    public bool CanWork()
    {
        return false;
    }

    public ToolWorkResult Work()
    {
        return (new ToolWorkResult
        {
            Trigger = null,
            DurationToStay = 0.1f
        });
    }


    [PunRPC]
    private void SetItemRPC(int playerId, int itemId)
    {
        HoldingItem = PhotonView.Find(itemId).GetComponent<Item>();
        HoldingItem.transform.SetParent(this.transform);
        HoldingItem.transform.position = ProductPoint.position;
        HoldingItem.transform.up = ProductPoint.up;
        StartCoroutine(ItemRemoveRoutine(playerId));
    }

    private IEnumerator ItemRemoveRoutine(int playerid)
    {
        yield return new WaitForSeconds(3);
        Destroy(HoldingItem);
        HoldingItem = null;
        recipeManager.FulfillRecipe();
        //_scoreManager.AddScore(, 1);
        

    }

}
