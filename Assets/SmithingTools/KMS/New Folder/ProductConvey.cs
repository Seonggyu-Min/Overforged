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

    private Item HoldingItem;




    void Awake()
    {
        //UI.enabled = false;

    }


    public bool CanTransferItem(ToolTransferArgs args)
    {
        bool result = false;
        if (HoldingItem == null && args.ItemToGive != null)
        {
            if (args.ItemToGive is ProductItem item)
            {
                result = recipeManager.Check(item.Data as ProductItemData, item.Ore, item.Wood);
            }
        }
        return result;
    }

    public ToolTransferResult Transfer(ToolTransferArgs args, bool fromNetwork = false)
    {
        if (args.ItemToGive != null)
        {
            StartCoroutine(ItemRemoveRoutine(args));
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

    public ToolWorkResult Work(bool fromNetwork = false)
    {
        return (new ToolWorkResult
        {
            Trigger = null,
            DurationToStay = 0.1f
        });
    }


    [PunRPC]
    private void SetItemRPC(int itemId)
    {
        HoldingItem = PhotonView.Find(itemId).GetComponent<Item>();
        HoldingItem.transform.SetParent(ProductPoint);
        HoldingItem.transform.position = ProductPoint.position;
        HoldingItem.transform.up = ProductPoint.up;
    }

    private IEnumerator ItemRemoveRoutine(ToolTransferArgs args)
    {
        int id = args.ItemToGive.GetComponent<PhotonView>().ViewID;
        photon.RPC("SetItemRPC", RpcTarget.All, id);
        _scoreManager.AddScore(PhotonNetwork.LocalPlayer, 1);
        yield return new WaitForSeconds(3);
        photon.RPC("DestroyRPC", RpcTarget.All);
        recipeManager.FulfillRecipe();


    }
    [PunRPC]

    private void DestroyRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(HoldingItem.GetComponent<PhotonView>());

        }
        HoldingItem = null;
    } 

}
