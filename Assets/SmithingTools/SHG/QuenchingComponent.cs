using System;
using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;
using Zenject;

namespace SHG
{
  public class QuenchingComponent : SmithingToolComponent
  {

    [Inject]
    IAudioLibrary audioLibrary;
    [SerializeField] [Required()]
    SmithingToolData quenchingToolData;
    [SerializeField] 
    QuenchingTool quenchingTool;
    [SerializeField]
    Transform materialPosition;
    [SerializeField] [Required()]
    ToonWater toonWater;
    [SerializeField] [Required()]
    ParticleSystem vaporParticle;
    [SerializeField] [Required()]
    MeshRenderer[] renderers;

//    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel), nameof(tempLabel))]
//    Void uiGroup;
//    [SerializeField] [HideProperty]
//    Canvas uiCanvas;
//    [SerializeField] [HideProperty]
//    Image itemImage;
//    [SerializeField] [HideProperty]
//    TMP_Text itemNameLabel;
//    [SerializeField] [HideProperty]
//    TMP_Text tempLabel;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color coolColor;
    [SerializeField]
    Color heatedColor;
    ObservableValue<(float current, float total)> progress;

    protected override SmithingTool tool => this.quenchingTool;
    protected override ISmithingToolEffecter effecter => this.quenchingEffecter;
    protected override Transform materialPoint => this.materialPosition;
    QuenchingEffecter quenchingEffecter;

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
//      if (this.uiCanvas.enabled && tool.HoldingItem == null) {
//        this.uiCanvas.enabled = false;
//      }
      if (tool.HoldingItem == null) {
        this.HideItemUI();
      }
      else {
        this.SetItemUI(tool.HoldingItem);
      }
      if (tool.InteractionToTrigger == SmithingTool.InteractionType.ReceivedItem) {
        this.highlighter.HighlightColor = this.heatedColor;
        this.quenchingEffecter.TriggerWorkEffect();
        this.audioLibrary.PlayRandomSfx("quench");
      } 
      else if (tool.InteractionToTrigger == SmithingTool.InteractionType.ReturnItem) {
        this.highlighter.HighlightColor = this.normalColor;
      }
    }

    void HideItemUI()
    {
      this.HideProgressUI();
    }

    void SetItemUI(Item item)
    {
      this.ShowProgressUI();
//      this.itemImage.sprite = item.Data.Image;   
//      this.itemNameLabel.text = item.Data.Name;
//      if (!this.uiCanvas.enabled) {
//        this.uiCanvas.enabled = true;
//      }
    }

    void OnFinished()
    {
      this.highlighter.HighlightColor = this.coolColor;
    }

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
      this.progress.Value = (this.quenchingTool.Progress, 1f);
//      if (this.uiCanvas.enabled) {
//        this.tempLabel.text = $"temp: {this.quenchingTool.Temparature}";
//      }
    }

    protected override void Awake()
    {
      base.Awake();
      this.highlighter = new GameObjectHighlighter(
        Array.ConvertAll<Renderer, Material>(
          this.renderers, renderer => renderer.material));
      for (int i = 0; i < this.renderers.Length; i++) {
        this.renderers[i].material = this.highlighter.HighlightedMaterials[i]; 
      }
      this.quenchingTool = new QuenchingTool(this.quenchingToolData);
      this.quenchingTool.BeforeInteract += this.BeforeInteract;
      this.quenchingTool.AfterInteract += this.AfterInteract;
      this.quenchingTool.OnFinished += this.OnFinished;
      this.quenchingEffecter = new QuenchingEffecter(
        quenchingTool: this.quenchingTool,
        toonWater: this.toonWater,
        vaporParticle: this.vaporParticle,
        materialPoint: this.materialPoint
        );
      //this.uiCanvas.enabled = false;
      this.progress = new ((0f, 1f));
      this.progressUI.WatchingFloatValue = this.progress;
    }
  }
}
