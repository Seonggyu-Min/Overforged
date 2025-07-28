using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;

namespace SHG
{

  public class FurnaceComponent : SmithingToolComponent
  {
    [SerializeField] [Required()]
    SmithingToolData furnaceData;
    [SerializeField] 
    Furnace furnace;
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
    Color ignitedColor;
    MeshRenderer meshRenderer;
    bool isIgnited;

    protected override SmithingTool tool => this.furnace;

    void BeforeInteract(SmithingTool tool)
    {
      if (tool != this.furnace) {
        return; 
      }
      Debug.Log($"Before Interact");
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.furnace) {
        return; 
      }
      Debug.Log($"After Interact");
      if (this.uiCanvas.enabled && tool.HoldingItem == null) {
        this.uiCanvas.enabled = false;
      }
      else if (!this.uiCanvas.enabled && tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
      }
      if (this.isIgnited != this.furnace.IsIgnited) {
        this.isIgnited = this.furnace.IsIgnited;
        this.meshRenderer.material.color = this.isIgnited ? 
          this.ignitedColor: this.normalColor;
      } 
    }

    void SetItemUI(Item item)
    {
      this.itemImage.sprite = item.Data.Image;   
      this.itemNameLabel.text = item.Data.Name;
      this.uiCanvas.enabled = true;
    }

    void OnFinished()
    {
      this.itemNameLabel.text += " (done)";
    }

    void Awake()
    {
      this.furnace = new Furnace(this.furnaceData);
      this.furnace.BeforeInteract += this.BeforeInteract;
      this.furnace.AfterInteract += this.AfterInteract;
      this.furnace.OnFinished += this.OnFinished;
      this.uiCanvas.enabled = false;
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      this.furnace.OnUpdate(Time.deltaTime);
      this.tempLabel.text = $"Temp: {this.furnace.Temparature}";
      this.itemProgressLabel.text = $"Progress: {this.furnace.Progress * 100}%";
    }
  }
}
