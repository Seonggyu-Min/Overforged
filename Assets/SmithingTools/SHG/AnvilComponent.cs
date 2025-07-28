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

    [SerializeField] [VerticalGroup(10f, true, nameof(sparkPrefab), nameof(sparkPoint))]
    Void effecterGroup;
    [SerializeField] [HideInInspector, Required]
    GameObject sparkPrefab;
    [SerializeField] [HideInInspector, Required]
    Transform sparkPoint;

    protected override SmithingTool tool => this.anvil;
    protected override ISmithingToolEffecter effecter => this.anvilEffecter;
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
          this.highlighter.HighlightedMaterial.color = this.interactColor;
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
        sparkPool: sparkPool,
        sparkPoint: this.sparkPoint);
      this.uiCanvas.enabled = false;
    }

    void OnInteractionTriggered(SmithingTool.InteractionType interactionType)
    {
      this.highlighter.HighlightedMaterial.color = this.normalColor;
      if (interactionType == SmithingTool.InteractionType.Work) {
        this.effecter.TriggerWorkEffect();
      }
    }
  }
}
