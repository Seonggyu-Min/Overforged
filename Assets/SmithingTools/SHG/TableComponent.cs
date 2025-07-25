using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{

  [RequireComponent(typeof(MeshRenderer))]
  public class TableComponent: MonoBehaviour, IInteractableTool
  {
    [SerializeField]
    WoodTable woodTable;
    [SerializeField]
    SmithingToolData woodTableData;
    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel), nameof(itemProgressLabel))]
    Void uiGroup;
    [SerializeField] [HideProperty]
    Canvas uiCanvas;
    [SerializeField] [HideProperty]
    Image itemImage;
    [SerializeField] [HideProperty]
    TMP_Text itemNameLabel;
    [SerializeField] [HideProperty]
    TMP_Text itemProgressLabel;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;
    MeshRenderer meshRenderer;

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
      if (this.uiCanvas.enabled && tool.HoldingItem == null) {
        this.uiCanvas.enabled = false;
      }
      else if (!this.uiCanvas.enabled && tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
      }
      if (tool.HoldingItem != null) {
        this.UpdateProgress();
      }
    }

    void SetItemUI(Item item)
    {
      this.itemImage.sprite = item.Data.Image;   
      this.itemNameLabel.text = item.Data.Name;
      this.uiCanvas.enabled = true;
    }

    void UpdateProgress()
    {
      this.itemProgressLabel.text = $"Progress: {this.woodTable.Progress * 100}%";
    }

    void Awake()
    {
      this.woodTable = new WoodTable(this.woodTableData);
      this.woodTable.BeforeInteract += this.BeforeInteract;
      this.woodTable.AfterInteract += this.AfterInteract;
      this.woodTable.OnInteractionTriggered += this.OnInteractionTriggered;
      this.uiCanvas.enabled = false;
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

    public bool CanTransferItem(ToolTransferArgs args)
    {
      bool canTransfer = this.woodTable.CanTransferItem(args);
      Debug.Log($"{nameof(CanTransferItem)} : {canTransfer}");
      return (canTransfer);
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
      var result = this.woodTable.Transfer(args);
      Debug.Log($"{nameof(Transfer)} result: {result}");
      return (result);
    }

    public bool CanWork()
    {
      bool canwork = this.woodTable.CanWork();
      Debug.Log($"{nameof(canwork)}: {canwork}");
      return (canwork);
    }

    public ToolWorkResult Work()
    {
      var result = this.woodTable.Work();
      Debug.Log($"{nameof(Work)} result: {result}");
      return (result);
    }
  }
}
