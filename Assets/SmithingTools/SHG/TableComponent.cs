#define LOCAL_TEST
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;
using Zenject;

namespace SHG
{
  [RequireComponent(typeof(MeshRenderer), typeof(Animator))]
  public class TableComponent : SmithingToolComponent
  {
    [Inject]
    IAudioLibrary audioLibrary;
    [SerializeField] [ReadOnly]
    WoodTable woodTable;
    [SerializeField]
    SmithingToolData woodTableData;
    [SerializeField] [ReadOnly]
    CraftTable craftTable;
    [SerializeField]
    CraftTableData craftTableData;
    [SerializeField] [Required()]
    Transform materialPosition;

    [SerializeField] 
    MeshRenderer model;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;

    [SerializeField]
    [VerticalGroup(10f, true, nameof(sawDustParticle), nameof(confettiParticle))]
    Void effecterGroup;
    [SerializeField]
    [Required(), HideProperty]
    ParticleSystem sawDustParticle;
    [SerializeField]
    [Required(), HideProperty]
    ParticleSystem confettiParticle;
    Animator animator;
    TableEffecter tableEffecter;
    ObservableValue<(float current, float total)> progress;

    List<string> materialNames;
    public override Item HoldingItem {
      get {
        if (this.woodTable.HoldingMaterial != null) {
          return (this.woodTable.HoldingMaterial);
        }
        return (this.craftTable.HoldingMaterial);
      }
    }

    IInteractableTool CurrentWorkingTool
    {
      get => this.currentWorkingTool;
      set {
        this.currentWorkingTool = value;
      }
    }
    protected override SmithingTool tool => this.woodTable;

    protected override ISmithingToolEffecter effecter => this.tableEffecter;

    protected override Transform materialPoint => this.materialPosition;

    [SerializeField] [ReadOnly]
    IInteractableTool currentWorkingTool;

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null &&
        this.CurrentWorkingTool == this.woodTable) {
        bool canWoodTableTransfer = this.woodTable.CanTransferItem(args);
        if (canWoodTableTransfer) {
          return (true);
        }
        else {
          bool isCraftTableTansfer = this.craftTable.CanTransferItem(args);
          return (isCraftTableTansfer);
        }
      }
      else if (this.CurrentWorkingTool != null) {
        return (this.CurrentWorkingTool.CanTransferItem(args));
      }
      else {
        bool canWoodTableTransfer = this.woodTable.CanTransferItem(args);
        bool canCraftTableTransfer = this.craftTable.CanTransferItem(args);
        return (canWoodTableTransfer || canCraftTableTransfer);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.transform.SetParent(this.transform);
        //TODO: Item position
        args.ItemToGive.transform.localPosition = Vector3.up;
        this.itemUI.AddImage(args.ItemToGive.Data.Image);
        if (this.CurrentWorkingTool == this.woodTable &&
          !this.woodTable.CanTransferItem(args) &&
          this.woodTable.HoldingMaterial != null &&
          this.craftTable.CanTransferItem(args)) {
          var result = this.MoveMaterialToCraftTable(args);
          this.OnTransfered?.Invoke(this, args, result);
          return (result);
        }
      }
      if (this.CurrentWorkingTool != null) {
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{nameof(Transfer)} result: {result}");
        if (result.IsDone) {
          this.CurrentWorkingTool = null;
          this.itemUI.SubAllImage();
        }
        else if (this.CurrentWorkingTool == this.craftTable &&
          this.craftTable.HoldingMaterials.Count == 0) {
          this.CurrentWorkingTool = null;
          this.itemUI.SubAllImage();
        }
        else {
          this.itemUI.SubImage();
        }
        this.OnTransfered?.Invoke(this, args, result);
        return (result);
      }
      else {
        if (this.woodTable.CanTransferItem(args)) {
          this.CurrentWorkingTool = this.woodTable;
        }
        else if (this.craftTable.CanTransferItem(args)) {
          this.CurrentWorkingTool = this.craftTable;
        }
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{this.CurrentWorkingTool} {nameof(Transfer)} result: {result}");
        this.OnTransfered?.Invoke(this, args, result);
        return (result);
#if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(TableComponent)} is not able Transfer"));
#else
        return ( new ToolTransferResult {} );
#endif
      }
    }

    ToolTransferResult MoveMaterialToCraftTable(ToolTransferArgs args)
    {
      MaterialItem ItemToGive = args.ItemToGive as MaterialItem;
      args.ItemToGive = null;
      var result = this.woodTable.Transfer(args);
      this.craftTable.Transfer(
        new ToolTransferArgs {
        ItemToGive = result.ReceivedItem as MaterialItem,
        PlayerNetworkId = args.PlayerNetworkId
      });
      args.ItemToGive = ItemToGive;
      this.CurrentWorkingTool = this.craftTable;
      return (this.craftTable.Transfer(args));
    }

    public override bool CanWork()
    {
      if (this.CurrentWorkingTool != null) {
        return (this.CurrentWorkingTool.CanWork());
      }
      return (false);
    }

    public override ToolWorkResult Work()
    {
      if (this.CurrentWorkingTool != null) {
        var result = this.CurrentWorkingTool.Work();
        if (this.CurrentWorkingTool == this.woodTable) {
          this.tableEffecter.TriggerWorkEffect();
        }
        else if (this.CurrentWorkingTool == this.craftTable) {
          this.animator.SetTrigger("Craft");
        }
        Debug.Log($"{this.CurrentWorkingTool} {nameof(Work)} result: {result}");
        this.OnWorked?.Invoke(this, result);
        return (result);
      }
      else {
      #if UNITY_EDITOR
        throw new ApplicationException($"{nameof(TableComponent)} is not workable try ${nameof(CanWork)} first");
      #else
        return (new ToolWorkResult {});
      #endif
      }
    }

    void BeforeWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingMaterial}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("AfterInteract");
      Debug.Log($"tool holding item: {tool.HoldingMaterial}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      Debug.Log($"currentWorkingTool: {this.currentWorkingTool}");
      if (tool.HoldingMaterial != null) {
        this.SetItemUI(tool.HoldingMaterial);
        if (tool.InteractionToTrigger == SmithingTool.InteractionType.Work) {
          this.highlighter.HighlightColor = this.interactColor;
        }
      }
      else {
        this.HideItemUI();
      }
    }

    void OnWoodTableTriggered(SmithingTool.InteractionType interactionType)
    {
      this.highlighter.HighlightColor = this.normalColor;
    }

    void HideItemUI()
    {
      this.HideProgressUI();
    }

    void SetItemUI(Item item)
    {
      this.progress.Value = (this.woodTable.Progress, 1f);
      this.ShowProgressUI();
    }

    void OnCraftMaterialAdded(MaterialItem newMaterial)
    {
      this.materialNames.Add(newMaterial.Data.Name);
      newMaterial.transform.localPosition = new Vector3(
        0, this.craftTable.HoldingMaterials.Count * 0.5f, 0);
      this.UpdateMaterialLabel();
    }

    void OnCraftMaterialRemoved(MaterialItem removedMaterial)
    {
      this.materialNames.Remove(removedMaterial.Data.Name);
      this.UpdateMaterialLabel();
      if (this.craftTable.HoldingMaterials.Count == 0) {
        this.HideItemUI();
      }
    }

    void OnCraftProductCrafted(ProductItemData craftedProduct)
    {
      this.tableEffecter.TriggerWorkEffect();
      this.audioLibrary.PlayRandomSfx(
        soundName: "success",
        position: this.transform.position);
    }

    void OnCraftProductRemoved(ProductItemData removedProduct)
    {
      this.HideItemUI();
    }

    void OnCraftableChanged()
    {
      if (this.craftTable.CraftableProduct != null) {
      }
      else {
      }
    }

    void UpdateMaterialLabel()
    {
      var builder = new StringBuilder("Materials: ");
      foreach (var name in this.materialNames)
      {
        builder.Append($"{name}, ");
      }
    }

    protected override void HandleNetworkTransfer(object[] args)
    {
      var dict = args[0] as Dictionary<string, object>;
      int playerNetworkId = (int)dict[ToolTransferArgs.PLAYER_NETWORK_ID_KEY];
      if (dict.TryGetValue(
          ToolTransferArgs.ITEM_ID_KEY, out object itemId) &&
        itemId != null) {
        if (this.NetworkSynchronizer.TryFindComponentFromNetworkId(
            networId: (int)itemId,
            out MaterialItem foundItem
            )) {
          this.Transfer(new ToolTransferArgs {
            ItemToGive = foundItem,
            PlayerNetworkId = playerNetworkId
          });
        }
        #if UNITY_EDITOR
        else {
          Debug.LogError($"item not found for {args[0]}");
        }
        #endif
      }
      else {
        if (this.woodTable.HoldingMaterial != null) {
          this.woodTable.HoldingMaterial.transform.SetParent(null);
        }
        else if (this.craftTable.Product != null) {
          this.craftTable.Product.transform.SetParent(null);
        }
        else if (this.craftTable.HoldingMaterials.Count > 0) {
          this.craftTable.HoldingMaterials[this.craftTable.HoldingMaterials.Count - 1].transform.SetParent(null);
        }
        this.Transfer(new ToolTransferArgs {
          ItemToGive = null,
          PlayerNetworkId = playerNetworkId
        });
      }
    }

    protected override void Awake()
    {
      base.meshRenderer = model;
      base.Awake();
      this.woodTable = new WoodTable(this.woodTableData);
      this.woodTable.BeforeInteract += this.BeforeWoodTableInteract;
      this.woodTable.AfterInteract += this.AfterWoodTableInteract;
      this.woodTable.OnInteractionTriggered += this.OnWoodTableTriggered;
      this.craftTable = new CraftTable(
        data: this.craftTableData,
        productPoint: this.materialPoint,
        createProduct: this.CreateProduct);
      this.craftTable.OnMaterialAdded += this.OnCraftMaterialAdded;
      this.craftTable.OnMaterialRemoved += this.OnCraftMaterialRemoved;
      this.craftTable.OnProductCrafted += this.OnCraftProductCrafted;
      this.craftTable.OnProductRemoved += this.OnCraftProductRemoved;
      this.craftTable.OnCraftableChanged += this.OnCraftableChanged;
      this.tableEffecter = new TableEffecter(
        woodTable: this.woodTable,
        craftTable: this.craftTable,
        sawDustParticleSystem: this.sawDustParticle,
        confettiParticleSystem: this.confettiParticle,
        getCurrentTool: () => this.CurrentWorkingTool
        );
      this.materialNames = new();
      this.animator = this.GetComponent<Animator>();
      this.progress = new ((0f, 1f));
      this.progressUI.WatchingFloatValue = this.progress;
    }

    protected override void HandleNetworkWork(object[] args)
    {
      // TODO: handle work result
      var dict = args[0] as Dictionary<string, object>;
      this.Work();
      if (this.CurrentWorkingTool == this.woodTable) {
        this.woodTable.OnInteractionTriggered?.Invoke(this.woodTable.InteractionToTrigger);
      }
    }

    public override void OnRpc(string method, float latencyInSeconds, object[] args = null)
    {
      switch (method)
      {
        case nameof(Transfer):
          this.HandleNetworkTransfer(args);
          break;
        case nameof(Work):
          this.HandleNetworkWork(args);
          break;
        case nameof(SetProductData):
          this.SetProductData(args);
          break;
      }
    }

    ProductItem CreateProduct(ItemData itemData)
    {
      #if LOCAL_TEST
      var gameObject = Instantiate(
          Resources.Load<GameObject>("ProductItem"),
          position: this.materialPoint.position,
          rotation: Quaternion.identity);
        var productItem = gameObject.GetComponent<ProductItem>();
        productItem.Data = itemData;
        return (productItem);
      #else
      if (this.IsOwner) {
        var gameObject = PhotonNetwork.Instantiate(
          prefabName: "ProductItem", 
          position: this.materialPoint.position,
          rotation: Quaternion.identity
          );
        var productItem = gameObject.GetComponent<ProductItem>();
        productItem.Data = itemData;
        int itemId = gameObject.GetComponent<PhotonView>().ViewID;
        this.NetworkSynchronizer.SendRpc(
          sceneId: this.SceneId,
          method: nameof(this.SetProductData),
          args: new object[] { itemId }
          );
        return (productItem);
      }
      else {
        #if UNITY_EDITOR
        Debug.LogError($"{nameof(CreateProduct)}: {this} is not owned");
        #endif
        return (null);
      }
      #endif
    }

    void SetProductData(object[] args)
    {
      if (this.NetworkSynchronizer.TryFindComponentFromNetworkId<ProductItem>(networId: (int)args[0], out ProductItem productItem)) {
        productItem.Data = this.craftTable.CraftableProduct;
      }
    }
  }
}
