using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{

  [RequireComponent(typeof(MeshRenderer))]
  public class AnvilComponent : MonoBehaviour, IInteractableTool
  {
    [SerializeField]
    Anvil anvil;
    [SerializeField]
    SmithingToolData data;
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
      if (tool != this.anvil) {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.anvil) {
        return;
      }
      Debug.Log("AfterInteract result");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (this.uiCanvas.enabled && tool.HoldingItem == null) {
        this.uiCanvas.enabled = false;
      }
      else if (tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
        if (tool.InteractionToTrigger == SmithingTool.InteractionType.Work) {
          this.meshRenderer.material.color = this.interactColor;
        }
      }
    }

    void SetItemUI(Item item)
    {
      this.itemImage.sprite = item.Data.Image;   
      this.itemNameLabel.text = item.Data.Name;
      if (!this.uiCanvas.enabled) {
        this.uiCanvas.enabled = true;
      }
      this.itemProgressLabel.text = $"Progress: {this.anvil.Progress * 100}%";
    }

    void Awake()
    {
      this.anvil = new Anvil(this.data);
      this.anvil.BeforeInteract += this.BeforeInteract;
      this.anvil.AfterInteract += this.AfterInteract;
      this.anvil.OnInteractionTriggered += this.OnInteractionTriggered;
      this.uiCanvas.enabled = false;
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      this.anvil.OnUpdate(Time.deltaTime);
    }

    void OnInteractionTriggered(SmithingTool.InteractionType interactionType)
    {
      this.meshRenderer.material.color = this.normalColor;
    }

    public bool CanTransferItem(ToolTransferArgs args)
    {
      bool canTransfer = this.anvil.CanTransferItem(args);
      Debug.Log($"{nameof(CanTransferItem)}: {canTransfer}");
      return (canTransfer);
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.transform.SetParent(this.transform);
        args.ItemToGive.transform.localPosition = Vector3.up;
      }
      var result = this.anvil.Transfer(args);
      Debug.Log($"Transfer result: {result}");
      return (result);
    }

    public bool CanWork()
    {
      bool canWork = this.anvil.CanWork();
      Debug.Log($"canwork: {canWork}");
      return (canWork);
    }

    public ToolWorkResult Work()
    {
      var result = this.anvil.Work();
      Debug.Log($"{nameof(Work)} result: {result}");
      return (result);
    }
  }
}
