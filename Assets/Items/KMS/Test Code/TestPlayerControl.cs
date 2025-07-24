using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KMS;
using MIN;
using Photon.Pun;
using UnityEngine;

public class TestPlayerControl : MonoBehaviourPun
{
    private Rigidbody rigid;

    [SerializeField] GameObject item;
    [SerializeField] GameObject product;
    [SerializeField] ItemDataList itemList;

    [SerializeField] Transform hand;

    private MaterialItem m;
    private ProductItem p;

    private PhotonView itemPv;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameObject go = PhotonNetwork.Instantiate("MatItem", Vector3.zero, Quaternion.identity);
            itemPv = go.GetComponent<PhotonView>();
            int itemViewId = itemPv.ViewID;
            photonView.RPC("SetMatItem", RpcTarget.All, itemViewId);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            itemPv.RPC("Heat", RpcTarget.All);
            //m.Heat();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            itemPv.RPC("Cool", RpcTarget.All);
            //m.Cool();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            itemPv.RPC("ChangeToNext", RpcTarget.All);
            //m.ChangeToNext();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            GameObject go = PhotonNetwork.Instantiate("ProductItem", Vector3.zero, Quaternion.identity);
            itemPv = go.GetComponent<PhotonView>();
            int itemViewId = itemPv.ViewID;
            photonView.RPC("SetProdItem", RpcTarget.All, itemViewId);
        }

    }
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movedist = transform.forward * v * 0.1f;
        rigid.MovePosition(rigid.position + movedist);

        rigid.rotation = rigid.rotation * Quaternion.Euler(0, h, 0);
    }

    [PunRPC]
    private void SetMatItem(int itemviewId)
    {

        PhotonView itemView = PhotonView.Find(itemviewId);
        if (itemView != null)
        {
            itemPv = itemView;
            m = itemView.GetComponent<MaterialItem>();
            m.Data = itemList.list[3];
            m.Ore = OreType.Gold;
            itemView.transform.SetParent(hand);
            itemView.transform.position = hand.position;
        }
    }
    [PunRPC]
    private void SetProdItem(int itemviewId)
    {
        
        PhotonView itemView = PhotonView.Find(itemviewId);
        if (itemView != null)
        {
            itemPv = itemView;
            p = itemView.GetComponent<ProductItem>();
            p.Data = itemList.productList[0];
            p.Ore = OreType.Copper;
            p.Wood = WoodType.Birch;
            itemView.transform.SetParent(hand);
            itemView.transform.position = hand.position;
        }
    }
}
