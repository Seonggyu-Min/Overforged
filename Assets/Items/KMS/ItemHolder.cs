using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


//상호작용 오브젝트의 위, 플레이어 손, 집게의 끝 등에 들어가서 아이템을 아이디로 추적해서 가져오는 용도
//상호작용 오브젝트의 경우 아이템이 들어올 때 이벤트로 호출이 될 것
//플레이어의 경우 상호작용 오브젝트에서 꺼낼 때 이벤트로
public class ItemHolder : MonoBehaviourPun
{

    [SerializeField] Transform HoldPivot;

    GameObject current;

    [PunRPC]
    private void GetItem(int itemviewId)
    {
        PhotonView itemView = PhotonView.Find(itemviewId);
        if (itemView != null)
        {
            current = itemView.gameObject;
            itemView.transform.SetParent(HoldPivot);
            itemView.transform.position = HoldPivot.position;
        }
    }

    private int GetItemViewId()
    {
        PhotonView itemPv = current.GetComponent<PhotonView>();
        int itemViewId = itemPv.ViewID;
        return itemViewId;

    }

    //상대측의 포톤뷰 아이디를 입력하여 내가 가진 아이템을 건내기
    //클라이언트 전역적으로 호출

    [PunRPC]
    public void GiveItemToOtherHolder(int otherViewId)
    {
        PhotonView otherView = PhotonView.Find(otherViewId);
        int itemId = GetItemViewId();
        otherView.GetComponent<ItemHolder>().GetItem(itemId);
        current = null;

    }

    //상대측 포톤뷰 아이디를 입력하여 가지고 있는 아이템을 가져오기
    //클라이언트 전역적으로 호출

    [PunRPC]

    public void TakeItemFromOtherHolder(int otherViewId)
    {
        PhotonView otherView = PhotonView.Find(otherViewId);
        ItemHolder itemHolder = otherView.GetComponent<ItemHolder>();
        int itemId = itemHolder.GetItemViewId();
        GetItem(itemId);
        itemHolder.current = null;
    }
}
