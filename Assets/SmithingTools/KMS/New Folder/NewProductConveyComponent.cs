using System.Collections;
using System.Collections.Generic;
using JJY;
using SHG;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using Zenject;
using SCR;

public class NewProductConveyComponent : SmithingToolComponent
{
    [SerializeField]
    NewProductConvey convey;
    [SerializeField]
    SmithingToolData data;
    [SerializeField]
    MeshRenderer model;
    [SerializeField]
    Transform productPosition;

    [SerializeField] OrderUI manager;
    [SerializeField] PhotonView photon;

    [SerializeField] GameObject good;

    [SerializeField] Transform goodPos;

    [Inject] MIN.IScoreManager _scoreManager;

    protected override SmithingTool tool => this.convey;
    protected override ISmithingToolEffecter effecter => null;

    protected override Transform materialPoint => this.productPosition;

    protected override void Update()
    {
        this.highlighter.OnUpdate(Time.deltaTime);
    }

    protected override void Awake()
    {
        //manager = GameObject.Find("InGameManager").GetComponent<InGameManager>().InGameUIManager.OrderUI;
        //this.meshRenderer = model;
        //if (this.meshRenderer != null)
        //{
        //    this.highlighter = new GameObjectHighlighter(
        //      new Material[] { this.meshRenderer.material });
        //    this.meshRenderer.material = this.highlighter.HighlightedMaterials[0];
        //}
        //this.convey = new NewProductConvey(this.data);
    }

    protected override void Start()
    {
        //manager = GameObject.Find("InGameManager").GetComponent<InGameManager>().InGameUIManager.OrderUI;
        this.meshRenderer = model;
        if (this.meshRenderer != null)
        {
            this.highlighter = new GameObjectHighlighter(
              new Material[] { this.meshRenderer.material });
            this.meshRenderer.material = this.highlighter.HighlightedMaterials[0];
        }
        this.convey = new NewProductConvey(this.data);

        this.IsOwner = true;
        if (TutorialManager.Instance == null)
        {
            manager = GameObject.Find("InGameManager").GetComponent<InGameManager>().InGameUIManager.OrderUI;
        }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args, bool fromNetwork = false)
    {
        var result = this.tool.Transfer(args);
        if (args.ItemToGive is ProductItem item)
        {
            if (TutorialManager.Instance != null)
            {
                Instantiate(good, goodPos.position, Quaternion.identity);
            }
            else if (manager.Check(item.Data as ProductItemData, item.Ore, item.Wood))
            {
                manager.FulfillRecipe();
                if (BotContext.Instance != null)
                {
                    BotContext.Instance.RemoveRecipe(item.Data as ProductItemData, item.Wood, item.Ore); // AI에 등록된 레시피 정보 삭제.
                }
                Instantiate(good, goodPos.position, Quaternion.identity);
            }
        }
        this.OnTransfered?.Invoke(this, args, result);
        StartCoroutine(ItemRemoveRoutine(args));
        return (result);
    }

    public override bool CanWork()
    {
        return false;
    }

    public override ToolWorkResult Work(bool fromNetwork = false)
    {
        return new ToolWorkResult();
    }

    [PunRPC]
    private void SetItemRPC(int itemId)
    {
        convey.HoldingProductItem = PhotonView.Find(itemId).GetComponent<ProductItem>();
        convey.HoldingProductItem.transform.SetParent(productPosition);
        convey.HoldingProductItem.transform.position = productPosition.position;
        convey.HoldingProductItem.transform.up = productPosition.up;
    }

    private IEnumerator ItemRemoveRoutine(ToolTransferArgs args)
    {
        int id = args.ItemToGive.GetComponent<PhotonView>().ViewID;
        photon.RPC("SetItemRPC", RpcTarget.All, id);
        yield return new WaitForSeconds(3);
        photon.RPC("DestroyRPC", RpcTarget.All);


    }
    [PunRPC]

    private void DestroyRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(convey.HoldingProductItem.GetComponent<PhotonView>());

        }
        convey.HoldingProductItem = null;
    }
}
