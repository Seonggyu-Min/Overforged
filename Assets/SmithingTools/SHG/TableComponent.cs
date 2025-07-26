using System;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{
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
    CraftTableData craftTableData;

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
      if (this.CurrentWorkingTool != null) {
        bool canTransfer = this.CurrentWorkingTool.CanTransferItem(args);
        Debug.Log($"{nameof(CanTransferItem)} {nameof(this.CurrentWorkingTool)} : {canTransfer}");
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
      if (this.CurrentWorkingTool != null) {
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{nameof(Transfer)} result: {result}");
        if (result.IsDone) {
          this.CurrentWorkingTool = null;
        }
        return (result);
      }
      else {
        if (this.woodTable.CanTransferItem(args)) {
          this.CurrentWorkingTool = this.woodTable;
          var result = this.CurrentWorkingTool.Transfer(args);
          Debug.Log($"{nameof(Transfer)} result: {result}");
          return (result);
        }
        else if (this.craftTable.CanTransferItem(args)) {
          this.CurrentWorkingTool = this.craftTable;
          var result = this.woodTable.Transfer(args);
          Debug.Log($"{nameof(Transfer)} result: {result}");
          return (result);
        }
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(TableComponent)} is not able Transfer"));
        #endif
        return ( new ToolTransferResult {} );
      }
    }

    public bool CanWork()
    {
      if (this.CurrentWorkingTool != null) {
        bool canWork = this.CurrentWorkingTool.CanWork();
        Debug.Log($"{nameof(canWork)}: {canWork}");

      }
      Debug.Log("No Tool is selected");
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

    void BeforeInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (this.woodTable.HoldingItem != null) {
        this.meshRenderer.material.color = this.interactColor;
      }
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.woodTable) {
        return;
      }
      Debug.Log("AfterInteract result");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (this.woodTableCanvas.enabled && tool.HoldingItem == null) {
        this.woodTableCanvas.enabled = false;
      }
      else if (!this.woodTableCanvas.enabled && tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
      }
      if (tool.HoldingItem != null) {
        this.UpdateProgress();
      }
    }

    void SetItemUI(Item item)
    {
      this.woodTableItemImage.sprite = item.Data.Image;   
      this.woodTableItemNameLabel.text = item.Data.Name;
      this.woodTableCanvas.enabled = true;
    }

    void UpdateProgress()
    {
      this.woodTableItemProgressLabel.text = $"Progress: {this.woodTable.Progress * 100}%";
    }

    void Awake()
    {
      this.woodTable = new WoodTable(this.woodTableData);
      this.woodTable.BeforeInteract += this.BeforeInteract;
      this.woodTable.AfterInteract += this.AfterInteract;
      this.woodTable.OnInteractionTriggered += this.OnInteractionTriggered;
      this.woodTableCanvas.enabled = false;
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      this.woodTable.OnUpdate(Time.deltaTime);
    }

    void OnInteractionTriggered(SmithingTool.InteractionType interactionType)
    {
      if (interactionType == SmithingTool.InteractionType.Work) {
        this.meshRenderer.material.color = this.normalColor;
      }
    }

  }
}
