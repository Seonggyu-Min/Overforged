using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SHG;

public class TestBox : MonoBehaviourPun, IInteractable
{
    [SerializeField] GameObject item;
    [SerializeField] GameObject product;
    [SerializeField] ItemDataList itemList;

    [SerializeField] Transform hand;

    private MaterialItem m;
    private ProductItem p;

    private PhotonView itemPv;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameObject go = PhotonNetwork.Instantiate("MatItem", Vector3.zero, Quaternion.identity);
            itemPv = go.GetComponent<PhotonView>();
            int itemViewId = itemPv.ViewID;
            photonView.RPC("SetMatItem", RpcTarget.All, itemViewId);
        }
    }
    public bool IsInteractable(PlayerInteractArgs args)
    {
        return true;
    }
    public ToolInteractArgs Interact(PlayerInteractArgs args)
    {
        return new ToolInteractArgs();
    }

}
