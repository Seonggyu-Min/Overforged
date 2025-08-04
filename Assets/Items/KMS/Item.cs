using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SHG;




public abstract class Item : MonoBehaviourPun, ICarryable
{

    [SerializeField] protected MaterialData matCatalog;
    [SerializeField] protected GameObject model;

    protected ItemData data;

    public virtual string Name { get; }

    protected GameObjectHighlighter highlighter;

    private BoxCollider collider;
    private Rigidbody rigid;

    protected bool isHot;
    public bool IsHot { get { return isHot; } }

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
            model.transform.localScale = data.Scale;
            model.transform.localPosition = new Vector3(data.xOffset, 0, 0);
            InitItemData(data);
        }
    }
    protected abstract void InitItemData(ItemData itemdata);

    public virtual void Hightlight(Color color)
    {
        highlighter.HighlightInstantly(color);
    }

    protected virtual void Awake()
    {
        collider = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
    }

    public void Go(Transform otherTrs)
    {
        transform.SetParent(otherTrs);
        transform.localPosition = Vector3.zero;
        collider.isTrigger = true;
        rigid.useGravity = false;
        rigid.isKinematic = true;
    }
    public void Come(Transform otherTrs, Transform myTrs)
    {
        Item other = otherTrs.GetComponentInChildren<Item>();
        other.Go(myTrs);
    }
    public void Abandon()
    {
        transform.SetParent(null);
        collider.isTrigger = false;
        rigid.useGravity = true;
        rigid.isKinematic = false;
    }

    void Update()
    {
        highlighter.OnUpdate(Time.deltaTime);
    }


}

