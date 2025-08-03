using System.Collections;
using UnityEngine;
using EditorAttributes;
using UnityEngine.Splines;

namespace SHG
{
  public class ConveyorBeltController : MonoBehaviour
  {
    public ConveyorBelt ConveyorBelt { get; private set; }
    public ConveyorBeltEffecter Effecter { get; private set; }
    public SplineContainer SplineContainer => this.splineContainer;
    public float Speed { get; private set; }
    [SerializeField] [Required]
    SmithingToolData conveyorBeltData;
    [SerializeField] [Required]
    SplineContainer splineContainer;
    [SerializeField] [Required]
    SplineInstantiate splineInstantiate;
    [SerializeField] [Required]
    GameObject conveyorBeltPrefab;
    [SerializeField] [Required]
    GameObject conveyorBoxPrefab;
    Material beltMaterial;
    [SerializeField] [Range(0f, 1f)]
    float movingSpeed;
    [SerializeField] [Range(0f, 1f)]
    float speedDelta;
    Coroutine powerRoutine;
    float beltOffset;
    WaitForSeconds powerDelay = new WaitForSeconds(0.1f);

    void Awake()
    {
      this.ConveyorBelt = new ConveyorBelt(
        data: this.conveyorBeltData,
        createBox: this.CreateBox); 
      this.ConveyorBelt.OnInteractionTriggered += this.OnInteractionTriggered;
      this.Effecter = new ConveyorBeltEffecter();
    }

    ConveyorBeltBox CreateBox()
    {
      return (
        Instantiate(this.conveyorBoxPrefab)
        .GetComponent<ConveyorBeltBox>());
    }

    // Start is called before the first frame update
    void Start()
    {
      GameObject beltObject = this.conveyorBeltPrefab.transform.Find("Belt").gameObject;
      this.beltMaterial = new Material(beltObject.GetComponent<MeshRenderer>().sharedMaterial);
      this.splineInstantiate.itemsToInstantiate[0].Prefab = null;
      this.splineInstantiate.Clear();
      this.splineInstantiate.itemsToInstantiate[0].Prefab = this.conveyorBeltPrefab;
      this.splineInstantiate.UpdateInstances();
      var root = this.splineInstantiate.gameObject.transform.GetChild(0);
      foreach (var child in root.transform) {
        if (child is Transform childTransform) {
          childTransform.Find("Belt").GetComponent<MeshRenderer>().material = this.beltMaterial;
        }
      }
    }

    void Update()
    {
      if (this.Speed > 0f) {
        this.beltOffset += this.Speed * Time.deltaTime;
        this.beltMaterial.SetFloat("_Offset", this.beltOffset);
      }
    }

    [Button]
    void TurnOn()
    {
      if (this.powerRoutine != null) {
        this.StopCoroutine(this.powerRoutine);
      }
      this.powerRoutine = this.StartCoroutine(this.TurnOnRoutine());
    }

    [Button]
    void TurnOff()
    {
      if (this.powerRoutine != null) {
        this.StopCoroutine(this.powerRoutine);
      }
      this.powerRoutine = this.StartCoroutine(this.TurnOffRoutine());
    }

    IEnumerator TurnOnRoutine()
    {
      while (this.Speed < this.movingSpeed) {
        this.Speed += this.speedDelta * Time.deltaTime;
        foreach (var box in this.ConveyorBelt.AllItemBox.Keys) {
          box.SetSpeed(this.Speed); 
        }
        yield return (this.powerDelay);
      }
    }

    IEnumerator TurnOffRoutine()
    {
      while (this.Speed >= 0) {
        this.Speed -= this.speedDelta * Time.deltaTime;
        foreach (var box in this.ConveyorBelt.AllItemBox.Keys) {
          box.SetSpeed(this.Speed); 
        }
        yield return (this.powerDelay);
      }
    }

    void OnInteractionTriggered(SmithingTool.InteractionType interactionType)
    {
      switch (interactionType)
      {
        case (SmithingTool.InteractionType.Work): 
          if (this.ConveyorBelt.IsPowerOn) {
            this.TurnOn();
          }
          else {
            this.TurnOff();
          }
          break;
      }
    }
  }
}
