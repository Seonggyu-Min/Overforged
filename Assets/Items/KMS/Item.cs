using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;



namespace KMS
{

    public class Item : MonoBehaviour, ICarryable
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
        //플레이어의 손 위치로 해당 아이템을 옮긴다.
        public void Carry(Transform playerHand)
        {
            transform.SetParent(playerHand);
        }

    }
}

