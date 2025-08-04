using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;

namespace SHG
{
  [RequireComponent(typeof(MeshRenderer))]
  public class AnvilComponent : SmithingToolComponent
  {
    [SerializeField]
    Anvil anvil;
    [SerializeField]
    SmithingToolData data;
    [SerializeField]
    MeshRenderer model;
    [SerializeField] [Required]
    Transform materialPosition;

    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;

    [SerializeField] [VerticalGroup(10f, true, nameof(sparkPrefab))]
    Void effecterGroup;
    [SerializeField] [HideInInspector, Required]
    GameObject sparkPrefab;

    protected override SmithingTool tool => this.anvil;
    protected override ISmithingToolEffecter effecter => this.anvilEffecter;

    protected override Transform materialPoint => this.materialPosition;
    ObservableValue<(float current, float total)> progress;

    AnvilEffecter anvilEffecter;

    void BeforeInteract(SmithingTool tool)
    {
      if (tool != this.anvil)
      {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingMaterial}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterInteract(SmithingTool tool)
    {
      if (tool != this.anvil) {
        return;
      }
      Debug.Log("AfterInteract result");
      Debug.Log($"tool holding item: {tool.HoldingMaterial}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      if (tool.HoldingMaterial == null) {
        this.HideItemUI();
      }
      else {
        this.SetItemUI(tool.HoldingMaterial);
        if (tool.InteractionToTrigger == SmithingTool.InteractionType.Work) {
          this.highlighter.HighlightColor = this.interactColor;
        }
      }
    }

    void HideItemUI()
    {
      this.HideProgressUI();
    }


    void SetItemUI(Item item)
    {
      this.ShowProgressUI();
      this.progress.Value = (this.anvil.Progress, 1f);
    }

    protected override void Awake()
    {
      this.meshRenderer = model;
      base.Awake();
      this.anvil = new Anvil(this.data);
      this.anvil.BeforeInteract += this.BeforeInteract;
      this.anvil.AfterInteract += this.AfterInteract;
      this.anvil.OnInteractionTriggered += this.OnInteractionTriggered;
      var sparkPool = new MonoBehaviourPool<SimplePooledObject>(
        poolSize: 10,
        prefab: this.sparkPrefab);
      this.anvilEffecter = new AnvilEffecter(
        anvil: this.anvil,
        sparkPool: sparkPool);
      this.progress = new ((0f, 1f));
      this.progressUI.WatchingFloatValue = this.progress;
    }

    void OnInteractionTriggered(SmithingTool.InteractionType interactionType)
    {
      this.highlighter.HighlightColor = this.normalColor;
      if (interactionType == SmithingTool.InteractionType.Work) {
        this.effecter.TriggerWorkEffect();
      }
    }
  }
}
