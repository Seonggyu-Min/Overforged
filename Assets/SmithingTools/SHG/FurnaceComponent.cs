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
    [SerializeField]
    FurnaceEffecter effecter;
    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel), nameof(itemProgressLabel), nameof(tempLabel))]
    Void uiGroup;
    [SerializeField] [HideInInspector]
    Canvas uiCanvas;
    [SerializeField] [HideInInspector]
    Image itemImage;
    [SerializeField] [HideInInspector]
    TMP_Text itemNameLabel;
    [SerializeField] [HideInInspector]
    TMP_Text itemProgressLabel;
    [SerializeField] [HideInInspector]
    TMP_Text tempLabel;
    bool isIgnited;

    [SerializeField] [VerticalGroup(10f, true, nameof(HightlightColor), nameof(normalColor), nameof(ignitedColor))]
    Void colorGroup;
    [SerializeField] [HideInInspector]
    Color normalColor;
    [SerializeField] [HideInInspector]
    Color ignitedColor;
    [SerializeField] [HideInInspector]
    public Color HightlightColor;
    protected override SmithingTool tool => this.furnace;

    [SerializeField] [VerticalGroup(10f, true, nameof(HightlightColor), nameof(normalColor), nameof(ignitedColor))]
    Void effecterGroup;
    [SerializeField] [HideInInspector]
    ParticleSystem fireParticle;

    [Button]
    void TurnOnFire()
    {
      this.effecter.TurnOn();
    }

    [Button]
    void TurnOffFire()
    {
      this.effecter.TurnOff();
    }

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
        this.highlighter.HighlightedMaterial.color = this.isIgnited ? 
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

    protected override void Awake()
    {
      base.Awake();
      this.furnace = new Furnace(this.furnaceData);
      this.furnace.BeforeInteract += this.BeforeInteract;
      this.furnace.AfterInteract += this.AfterInteract;
      this.furnace.OnFinished += this.OnFinished;
      this.effecter = new FurnaceEffecter(this.furnace);
      this.uiCanvas.enabled = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
      this.tempLabel.text = $"Temp: {this.furnace.Temparature}";
      this.itemProgressLabel.text = $"Progress: {this.furnace.Progress * 100}%";
    }
  }
}
