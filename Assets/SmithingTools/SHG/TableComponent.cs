using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{
  using Craft = TestCraft;
  using CraftTableData = TestCraftData;

  [RequireComponent(typeof(MeshRenderer))]
  public class TableComponent: MonoBehaviour, IInteractableTool
  {
    [SerializeField]
    WoodTable woodTable;
    [SerializeField]
    SmithingToolData woodTableData;
    [SerializeField]
    CraftTable craftTable;
    [SerializeField]
    CraftTableData[] craftListData;

    [SerializeField] [VerticalGroup(10f, true, nameof(woodTableCanvas), nameof(woodTableItemImage), nameof(woodTableItemNameLabel), nameof(woodTableItemProgressLabel))]
    Void woodTableGroup;
    [SerializeField] [HideProperty]
    Canvas woodTableCanvas;
    [SerializeField] [HideProperty]
    Image woodTableItemImage;
    [SerializeField] [HideProperty]
    TMP_Text woodTableItemNameLabel;
    [SerializeField] [HideProperty]
    TMP_Text woodTableItemProgressLabel;

    [SerializeField] [VerticalGroup(10f, true, nameof(craftTableCanvas), nameof(craftProductImage), nameof(craftProductNameLabel), nameof(craftMaterialListLabel))]
    Void craftTableGroup;
    [SerializeField] [HideProperty]
    Canvas craftTableCanvas;
    [SerializeField] [HideProperty]
    Image craftProductImage;
    [SerializeField] [HideProperty]
    TMP_Text craftProductNameLabel;
    [SerializeField] [HideProperty]
    TMP_Text craftMaterialListLabel;

    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;
    MeshRenderer meshRenderer;

    List<string> materialNames;

    IInteractableTool CurrentWorkingTool
    {
      get => this.currentWorkingTool;
      set {
        this.currentWorkingTool = value;
      }
    }
    IInteractableTool currentWorkingTool;

    public bool CanTransferItem(ToolTransferArgs args)
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
        bool canTransfer = this.CurrentWorkingTool.CanTransferItem(args);
        Debug.Log($"{nameof(CanTransferItem)} {this.CurrentWorkingTool} : {canTransfer}");
        return (canTransfer);
      }
      else {
        bool canWoodTableTransfer = this.woodTable.CanTransferItem(args);
        Debug.Log($"{nameof(CanTransferItem)} {nameof(WoodTable)} : {canWoodTableTransfer}");
        bool canCraftTableTransfer = this.craftTable.CanTransferItem(args);
        Debug.Log($"{nameof(CanTransferItem)} {nameof(CraftTable)} : {canCraftTableTransfer}");
        return (canWoodTableTransfer || canCraftTableTransfer);
      }
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.transform.SetParent(this.transform);
        args.ItemToGive.transform.localPosition = Vector3.up;
        if (this.CurrentWorkingTool == this.woodTable &&
          !this.woodTable.CanTransferItem(args) &&
          this.woodTable.HoldingItem != null &&
          this.craftTable.CanTransferItem(args)) {
          return (this.MoveMaterialToCraftTable(args));      
        }
      }
      if (this.CurrentWorkingTool != null) {
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{nameof(Transfer)} result: {result}");
        if (result.IsDone) {
          this.CurrentWorkingTool = null;
        }
        if (this.CurrentWorkingTool == this.craftTable &&
          this.craftTable.HoldingMaterials.Count == 0) {
          this.CurrentWorkingTool = null;
        }
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
        return (result);
#if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(TableComponent)} is not able Transfer"));
        #endif
        return ( new ToolTransferResult {} );
      }
    }

    ToolTransferResult MoveMaterialToCraftTable(ToolTransferArgs args)
    {
      MaterialItem ItemToGive = args.ItemToGive;
      args.ItemToGive = null;
      var result = this.woodTable.Transfer(args);
      this.craftTable.Transfer(new ToolTransferArgs {
          ItemToGive = result.ReceivedItem as MaterialItem,
          PlayerNetworkId = args.PlayerNetworkId
        });
      args.ItemToGive = ItemToGive;
      this.CurrentWorkingTool = this.craftTable;
      return (this.craftTable.Transfer(args));
    }

    public bool CanWork()
    {
      if (this.CurrentWorkingTool != null) {
        bool canWork = this.CurrentWorkingTool.CanWork();
        Debug.Log($"{nameof(canWork)}: {canWork}");
        return (canWork);
      }
      else {
        Debug.LogError("No Tool is selected");
      }
      return (false);
    }

    public ToolWorkResult Work()
    {
      if (this.CurrentWorkingTool != null) {
        var result = this.CurrentWorkingTool.Work();
        Debug.Log($"{nameof(Work)} result: {result}");
        return (result);
      }
      else {
        #if UNITY_EDITOR
        throw new ApplicationException($"{nameof(TableComponent)} is not workable try ${nameof(CanWork)} first");
        #endif
        return (new ToolWorkResult {});
      }
    }

    void BeforeWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("AfterInteract result");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
        if (tool.InteractionToTrigger == SmithingTool.InteractionType.Work) {
        this.meshRenderer.material.color = this.interactColor;
        }
      }
      else {
        this.woodTableCanvas.enabled = false;
      }
    }

    void OnWoodTableTriggered(SmithingTool.InteractionType interactionType)
    {
      this.meshRenderer.material.color = this.normalColor;
    }

    void SetItemUI(Item item)
    {
      this.woodTableItemImage.sprite = item.Data.Image;   
      this.woodTableItemNameLabel.text = item.Data.Name;
      this.woodTableItemProgressLabel.text = $"Progress: {this.woodTable.Progress * 100}%";
      if (!this.woodTableCanvas.enabled) {
        this.woodTableCanvas.enabled = true;
      }

    }

    void OnCraftMaterialAdded(MaterialItem newMaterial)
    {
      if (!this.craftTableCanvas.enabled) {
        this.craftTableCanvas.enabled = true;
      }
      this.materialNames.Add(newMaterial.Data.Name);
      newMaterial.transform.localPosition = new Vector3(
        0, this.craftTable.HoldingMaterials.Count * 0.5f, 0);
      this.UpdateMaterialLabel();
    }

    void OnCraftMaterialRemoved(MaterialItem removedMaterial)
    {
      this.materialNames.Remove(removedMaterial.Data.Name);
      this.UpdateMaterialLabel();
      if (this.craftTable.HoldingMaterials.Count == 0 && 
        this.craftTableCanvas.enabled) {
        this.craftTableCanvas.enabled = false; 
      }
    }

    void OnCraftProductCrafted(ProductItemData craftedProduct)
    {
      this.craftProductNameLabel.text = craftedProduct.Name; 
      this.craftProductImage.sprite = craftedProduct.Image;
    }

    void OnCraftProductRemoved(ProductItemData removedProduct)
    {
      this.craftProductNameLabel.text = "";
      this.craftProductImage.sprite = null;
      this.craftTableCanvas.enabled = false;
    }

    void OnCraftableChanged()
    {
      if (this.craftTable.CraftableProduct != null) {
        this.craftProductNameLabel.text = $"craftable: {this.craftTable.CraftableProduct.Name}";
      }
      else {
        this.craftProductNameLabel.text = "";
      }
    }

    void UpdateMaterialLabel()
    {
      var builder = new StringBuilder("Materials: ");
      foreach (var name in this.materialNames) {
         builder.Append($"{name}, "); 
      }
      this.craftMaterialListLabel.text = builder.ToString();
    }

    void Awake()
    {
      this.woodTable = new WoodTable(this.woodTableData);
      this.woodTable.BeforeInteract += this.BeforeWoodTableInteract;
      this.woodTable.AfterInteract += this.AfterWoodTableInteract;
      this.woodTable.OnInteractionTriggered += this.OnWoodTableTriggered;
      this.woodTableCanvas.enabled = false;
      var craftList = new Craft[this.craftListData.Length];
      for (int i = 0; i < craftList.Length; i++) {
        craftList[i] = new Craft(this.craftListData[i]); 
      }
      this.craftTable = new CraftTable(craftList);
      this.craftTable.OnMaterialAdded += this.OnCraftMaterialAdded;
      this.craftTable.OnMaterialRemoved += this.OnCraftMaterialRemoved;
      this.craftTable.OnProductCrafted += this.OnCraftProductCrafted;
      this.craftTable.OnProductRemoved += this.OnCraftProductRemoved;
      this.craftTable.OnCraftableChanged += this.OnCraftableChanged;
      this.meshRenderer = this.GetComponent<MeshRenderer>();
      this.materialNames = new();
      this.woodTableCanvas.enabled = false;
      this.craftTableCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
      this.woodTable.OnUpdate(Time.deltaTime);
    }

  }
}
