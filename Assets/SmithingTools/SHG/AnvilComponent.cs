using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
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
    MeshRenderer standRenderer;
    [SerializeField] [Required]
    Transform materialPosition;

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

    [SerializeField] [VerticalGroup(10f, true, nameof(sparkPrefab))]
    Void effecterGroup;
    [SerializeField] [HideInInspector, Required]
    GameObject sparkPrefab;

    protected override SmithingTool tool => this.anvil;
    protected override ISmithingToolEffecter effecter => this.anvilEffecter;

    protected override Transform materialPoint => this.materialPosition;

    AnvilEffecter anvilEffecter;

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
          this.highlighter.HighlightColor = this.interactColor;
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

    protected override void Awake()
    {
      this.meshRenderer = this.GetComponent<MeshRenderer>();
      this.highlighter = new GameObjectHighlighter(
        new Material[] { this.meshRenderer.material, this.standRenderer.material });
      this.meshRenderer.material = this.highlighter.HighlightedMaterials[0];
      this.standRenderer.material = this.highlighter.HighlightedMaterials[1];
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
      this.uiCanvas.enabled = false;
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
