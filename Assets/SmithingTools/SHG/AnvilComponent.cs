using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{
  using Item = TestItem;

  [RequireComponent(typeof(MeshRenderer))]
  public class AnvilComponent : MonoBehaviour, IInteractable
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

    public ToolInteractArgs Interact(PlayerInteractArgs args)
    {
      Debug.Log("Interact");
      return (this.anvil.Interact(args));
    }

    public bool IsInteractable(PlayerInteractArgs args)
    {
      bool isInteractable = this.anvil.IsInteractable(args);
      Debug.Log($"IsInteractable: {isInteractable}");
      return (isInteractable);
    }

    void BeforeInteract(SmithingTool tool, PlayerInteractArgs args)
    {
      if (tool != this.anvil) {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log(args);
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      this.meshRenderer.material.color = this.interactColor;
    }

    void AfterInteract(SmithingTool tool, ToolInteractArgs result)
    {
      if (tool != this.anvil) {
        return;
      }
      Debug.Log("AfterInteract result");
      Debug.Log(result);
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (this.uiCanvas.enabled && result.ReceivedItem != null) {
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

    void OnInteractionTriggered(IInteractable interactable)
    {
      if (System.Object.ReferenceEquals(this, interactable)) {
        this.meshRenderer.material.color = this.normalColor;
      }
    }
  }
}
