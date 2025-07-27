using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{
  public class QuenchingComponent : SmithingToolComponent
  {
    [SerializeField] [Required()]
    SmithingToolData quenchingToolData;
    [SerializeField] 
    QuenchingTool quenchingTool;
    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel), nameof(itemProgressLabel), nameof(tempLabel))]
    Void uiGroup;
    [SerializeField] [HideProperty]
    Canvas uiCanvas;
    [SerializeField] [HideProperty]
    Image itemImage;
    [SerializeField] [HideProperty]
    TMP_Text itemNameLabel;
    [SerializeField] [HideProperty]
    TMP_Text itemProgressLabel;
    [SerializeField] [HideProperty]
    TMP_Text tempLabel;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color coolColor;
    [SerializeField]
    Color heatedColor;
    MeshRenderer meshRenderer;

    protected override SmithingTool tool => this.quenchingTool;

    void BeforeInteract(SmithingTool tool)
    {
      if (tool != this.quenchingTool) {
        return ;
      } 
      Debug.Log($"Before Interact");
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.quenchingTool) {
        return; 
      }
      Debug.Log($"After Interact");
      if (this.uiCanvas.enabled && tool.HoldingItem == null) {
        this.uiCanvas.enabled = false;
      }
      if (tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
      }
      if (tool.InteractionToTrigger == SmithingTool.InteractionType.ReceivedItem) {
        this.meshRenderer.material.color = this.heatedColor;
      } 
      else if (tool.InteractionToTrigger == SmithingTool.InteractionType.ReturnItem) {
        this.meshRenderer.material.color = this.normalColor;
      }
    }

    void SetItemUI(Item item)
    {
      this.itemImage.sprite = item.Data.Image;   
      this.itemNameLabel.text = item.Data.Name;
      if (!this.uiCanvas.enabled) {
        this.uiCanvas.enabled = true;
      }
    }

    void OnFinished()
    {
      this.meshRenderer.material.color = this.coolColor;
    }

    // Update is called once per frame
    void Update()
    {
      this.quenchingTool.OnUpdate(Time.deltaTime);
      if (this.uiCanvas.enabled) {
        this.tempLabel.text = $"temp: {this.quenchingTool.Temparature}";
        this.itemProgressLabel.text = $"progress: {this.quenchingTool.Progress * 100}%";
      }
    }

    void Awake()
    {
      this.quenchingTool = new QuenchingTool(this.quenchingToolData);
      this.quenchingTool.BeforeInteract += this.BeforeInteract;
      this.quenchingTool.AfterInteract += this.AfterInteract;
      this.quenchingTool.OnFinished += this.OnFinished;
      this.uiCanvas.enabled = false;
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

  }
}
