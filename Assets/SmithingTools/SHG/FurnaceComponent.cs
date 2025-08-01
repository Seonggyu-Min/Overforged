using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;
using Zenject;

namespace SHG
{
  public class FurnaceComponent : SmithingToolComponent
  {
    [Inject]
    IAudioLibrary audioLibrary;
    [SerializeField] [Required()]
    SmithingToolData furnaceData;
    [SerializeField]
    Furnace furnace;
    [SerializeField]
    FurnaceEffecter furnaceEffecter;
    [SerializeField] [Required]
    Transform materialPosition;

//    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel))]
//    Void uiGroup;
//    [SerializeField] [HideInInspector]
//    Canvas uiCanvas;
//    [SerializeField] [HideInInspector]
//    Image itemImage;
//    [SerializeField] [HideInInspector]
//    TMP_Text itemNameLabel;
    bool isIgnited;
    [SerializeField] MeshRenderer model;

    [SerializeField] [VerticalGroup(10f, true, nameof(HightlightColor), nameof(normalColor), nameof(ignitedColor))]
    Void colorGroup;
    [SerializeField] [HideInInspector]
    Color normalColor;
    [SerializeField] [HideInInspector]
    Color ignitedColor;
    [SerializeField] [HideInInspector]
    public Color HightlightColor;
    protected override SmithingTool tool => this.furnace;

    protected override Transform materialPoint => this.materialPosition;
    protected override ISmithingToolEffecter effecter => this.furnaceEffecter;

    [SerializeField]
    [VerticalGroup(10f, true, nameof(fireParticle), nameof(sparkParticle))]
    Void effecterGroup;
    [SerializeField] [Required, HideInInspector]
    ParticleSystem fireParticle;
    [SerializeField] [Required, HideInInspector]
    ParticleSystem sparkParticle;
    SfxController burningSfx;
    ObservableValue<(float current, float total)> progress;

    [Button]
    void TurnOff()
    {
      if (this.furnace.IsIgnited)
      {
        this.furnace.TurnOff();
      }
      if (this.burningSfx != null)
      {
        this.burningSfx
          .Stop()
          .gameObject.SetActive(false);
        this.burningSfx = null;
      }
      //if (this.effecter.IsStateOn(ISmithingToolEffecter.State.Working)) {
      //  this.effecter.ToggleState(ISmithingToolEffecter.State.Working);
      //}
    }

    void BeforeInteract(SmithingTool tool)
    {
      if (tool != this.furnace)
      {
        return;
      }
      Debug.Log($"Before Interact");
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.furnace)
      {
        return;
      }
      Debug.Log($"After Interact");
      if (tool.HoldingItem != null) {
        this.SetItemUI(tool.HoldingItem);
      }
      else {
        this.HideItemUI();
      }
//      if (this.uiCanvas.enabled && tool.HoldingItem == null)
//      {
//        this.uiCanvas.enabled = false;
//      }
//      else if (!this.uiCanvas.enabled && tool.HoldingItem != null)
//      {
//        this.SetItemUI(tool.HoldingItem);
//      }
      if (this.isIgnited != this.furnace.IsIgnited)
      {
        this.isIgnited = this.furnace.IsIgnited;
        this.highlighter.HighlightColor = this.isIgnited ?
          this.ignitedColor : this.normalColor;
        if (this.isIgnited != this.effecter.IsStateOn(ISmithingToolEffecter.State.Working))
        {
          this.effecter.ToggleState(ISmithingToolEffecter.State.Working);
          this.audioLibrary.PlayRandomSfx(
            soundName: "ignite",
            position: this.transform.position);
          this.Invoke(nameof(PlayBurningSound), 2f);
        }
      }
    }

    void PlayBurningSound()
    {
      this.burningSfx = this.audioLibrary.PlayRandomSfx(
        soundName: "burning",
        position: this.transform.position)
        .SetLoop(true);
    }

    void HideItemUI()
    {
      this.HideProgressUI();
    }

    void SetItemUI(Item item)
    {
//      this.itemImage.sprite = item.Data.Image;
//      this.itemNameLabel.text = item.Data.Name;
//      this.uiCanvas.enabled = true;
      this.ShowProgressUI();
      this.progress.Value = (this.furnace.Progress, 1f);
    }

    void OnFinished()
    {
      //this.itemNameLabel.text += " (done)";
    }

    protected override void Awake()
    {
      base.meshRenderer = this.model;
      base.Awake();
      this.furnace = new Furnace(this.furnaceData);
      this.furnace.BeforeInteract += this.BeforeInteract;
      this.furnace.AfterInteract += this.AfterInteract;
      this.furnace.OnFinished += this.OnFinished;
      this.furnaceEffecter = new FurnaceEffecter(
        furnace: this.furnace,
        fireParticle: this.fireParticle,
        sparkParticle: this.sparkParticle);
      //this.uiCanvas.enabled = false;
      this.progress = new ((0f, 1f));
      this.progressUI.WatchingFloatValue = this.progress;
    }

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
      this.progress.Value = (this.furnace.Progress, 1f);
    }
  }
}
