using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using Photon.Pun;




public class Item : MonoBehaviourPun, ICarryable
{

    [SerializeField] protected MaterialData matCatalog;
    [SerializeField] protected GameObject model;

    protected ItemData data;


    //아이템이 참조할 아이템 스크립터블 오브젝트
    public ItemData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            InitItemData(data);
        }
    }
    protected virtual void InitItemData(ItemData itemdata)
    {
    }

    //아마도 플레이어 측에서 ICarryable 인터페이스를 getcomponent로 가져올 것.
    // 다만 도구와의 상호작용에서는 그냥 함수를 호출해도 괜찮을 것 같음.
    // 아무 곳에나 놓였을 경우 Icarryable을 받아와야 할 것 같다.

    public void Go(Transform otherTrs)
    {
        transform.position = otherTrs.position;
        transform.SetParent(otherTrs);
    }
    public void Come(Transform otherTrs, Transform myTrs)
    {
        Item other = otherTrs.GetComponentInChildren<Item>();
        other.Go(myTrs);
    }
    public void Abandon()
    {
        transform.SetParent(null);
    }


}

