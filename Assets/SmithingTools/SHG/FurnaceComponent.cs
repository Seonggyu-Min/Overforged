using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using Void = EditorAttributes.Void;
using Zenject;

namespace SHG
{
  public class FurnaceComponent : SmithingToolComponent
  {
    static readonly Vector3 SCALE_IN_FURNACE = new Vector3(0.5f, 0.5f, 0.5f);

    public bool IsIgnited => this.furnace.IsIgnited;
    public bool IsFinished => this.furnace.IsFinished;
    
    IAudioLibrary audioPlayer => (SingletonAudio.Instance );
    [SerializeField] [Required()]
    SmithingToolData furnaceData;
    [SerializeField]
    Furnace furnace;
    [SerializeField]
    FurnaceEffecter furnaceEffecter;
    [SerializeField] [Required]
    Transform materialPosition;
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
      if (tool.HoldingMaterial != null) {
        this.SetItemUI(tool.HoldingMaterial);
      }
      else {
        this.HideItemUI();
      }
      if (this.isIgnited != this.furnace.IsIgnited)
      {
        this.isIgnited = this.furnace.IsIgnited;
        this.highlighter.HighlightColor = this.isIgnited ?
          this.ignitedColor : this.normalColor;
        if (this.isIgnited != this.effecter.IsStateOn(ISmithingToolEffecter.State.Working))
        {
          this.effecter.ToggleState(ISmithingToolEffecter.State.Working);
          this.audioPlayer.PlayRandomSfx(
            soundName: "ignite",
            position: this.transform.position);
          this.Invoke(nameof(PlayBurningSound), 2f);
        }
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null) {
        args.ItemToGive.transform.localScale = SCALE_IN_FURNACE;
      }
      var result = base.Transfer(args);
      if (result.ReceivedItem != null) {
        result.ReceivedItem.transform.localScale = Vector3.one;
      }
      return (result);
    }

    void PlayBurningSound()
    {
      this.burningSfx = this.audioPlayer.PlayRandomSfx(
        soundName: "burning",
        position: this.transform.position)
        .SetLoop(true)
        .SetDistance(max: 5f)
        .Set3dBlend(1f);
    }

    void HideItemUI()
    {
      this.HideProgressUI();
    }

    void SetItemUI(Item item)
    {
      this.ShowProgressUI();
      this.progress.Value = (this.furnace.Progress, 1f);
    }

    void OnFinished()
    {
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
