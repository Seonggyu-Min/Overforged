using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class TestBoxComponent : MonoBehaviourPun, IInteractableTool
{
    [SerializeField] MaterialItemData data;
    [SerializeField] OreType ore;
    [SerializeField] WoodType wood;

    [Header("legacy")]
    [SerializeField] ItemDataList itemdata;

    private MaterialItem HoldingItem;

    [SerializeField] Canvas UI;

    [SerializeField] Transform up;


    void Awake()
    {
        //UI.enabled = false;
    }


    public bool CanTransferItem(ToolTransferArgs args)
    {
        return false;
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
        UI.enabled = false;
        //var item = HoldingItem;
        //this.HoldingItem = null;
        return (new ToolTransferResult
        {
            //ReceivedItem = item,
            IsDone = true
        });
    }

    public bool CanWork()
    {
        return true;
    }


    //상호작용 시 UI가 켜져있으면 끄고, 꺼져있으면 키고 못움직이도록 해보았습니다.

    public ToolWorkResult Work()
    {
        if (UI.enabled)
        {
            UI.enabled = false;
            return (new ToolWorkResult
            {
                Trigger = null,
                DurationToStay = 0.1f
            });

        }
        else
        {
            UI.enabled = true;
            return (new ToolWorkResult
            {
                Trigger = null,
                DurationToStay = float.MaxValue
            });
        }
    }
    private void GenerateItem(MaterialItemData data, OreType ore, WoodType wood)
    {
        GameObject go = PhotonNetwork.Instantiate("MatItem", up.position, Quaternion.identity);
        PhotonView itemPv = go.GetComponent<PhotonView>();
        int itemViewId = itemPv.ViewID;
        photonView.RPC("SetupItem", RpcTarget.All, itemViewId, data, ore, wood);

        //TestPlayerControl player = GameObject.Find("Player").GetComponent<TestPlayerControl>();
        //Transform hand = player.hand;
        //player.current = HoldingItem;
        //HoldingItem.Go(hand);
        //HoldingItem = null;
        //UI.enabled = false;
    }

    public GameObject CreateItem()
    {
        GameObject item = PhotonNetwork.Instantiate("MatItem", transform.position, Quaternion.identity);
        HoldingItem = item.GetComponent<MaterialItem>();
        HoldingItem.Data = data;
        HoldingItem.Ore = ore;
        HoldingItem.Wood = wood;
        return item;
    }

    public void ButtonClick(BoxButton btn)
    {
        GenerateItem(btn.data, btn.ore, btn.wood);
        //photonView.RPC("GenerateItem", RpcTarget.All, btn.data, btn.ore, btn.wood);

    }

    [PunRPC]

    public void SetupItem(int id, MaterialItemData data, OreType ore, WoodType wood)
    {
        PhotonView p = PhotonView.Find(id);
        if (p != null)
        {
            MaterialItem HoldingItem = p.GetComponent<MaterialItem>();
            HoldingItem.Data = data;
            if (ore != OreType.None) HoldingItem.Ore = ore;
            if (wood != WoodType.None) HoldingItem.Wood = wood;

        }

    }
}
