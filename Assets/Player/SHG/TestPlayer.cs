using System;
using System.Collections;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  [RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
  public class TestPlayer : MonoBehaviour
  {

    [SerializeField] [Range(1f, 3f)]
    float interactRadius;
    [SerializeField] [Range(1f, 10f)]
    float interactRange; 
    [SerializeField]
    TestMaterialItemData HoldingItemData;
    public Action<IInteractable> OnTriggerInteraction;
    IInteractable lastInteractable;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;
    float interactionDuration;
    Coroutine toolInteractionRoutine;
    MeshRenderer meshRenderer;

    public TestMaterialItem HoldingItem => this.HoldingItemData != null ?  new TestMaterialItem(this.HoldingItemData): null;

    bool IsTryingInteract()
    {
      return (Input.GetKeyDown(KeyCode.E));
    }

    bool IsTryingGrab()
    {
      return (Input.GetKeyDown(KeyCode.G));
    }

    bool TryFindInteratable(out IInteractable interactable)
    {
      #if UNITY_EDITOR
      Debug.DrawLine(
        start: this.transform.position,
        end: this.transform.position + this.transform.forward * this.interactRange,
        color: Color.blue,
        duration: 0.5f);
      #endif
      bool isHit = Physics.SphereCast(
        origin: this.transform.position,
        radius: this.interactRadius,
        direction: this.transform.forward,
        hitInfo: out RaycastHit hitInfo,
        maxDistance: this.interactRange);
      if (!isHit) {
        interactable = null;
        return (false);
      } 
      interactable = hitInfo.collider.GetComponent<IInteractable>();
      return (interactable != null);
    }

    void Update()
    {
      var movingInput = this.GetInput();
      if (movingInput != Vector2.zero) {
        this.Move(movingInput);
        this.Rotate(movingInput);
      }
      else {
        this.rb.velocity = Vector2.zero;
      }

      if (this.IsTryingInteract() &&
        this.TryFindInteratable(out IInteractable interactable)) {
        PlayerInteractArgs args = this.GetInteractArgs();
        if (interactable.IsInteractable(args)) {
          this.Interact(interactable, args);
        }
      }
    }

    PlayerInteractArgs GetInteractArgs()
    {
      return (new PlayerInteractArgs {
          CurrentHoldingItem = this.HoldingItem,
          PlayerNetworkId = 1
        });
    }

    void Interact(in IInteractable interactable, in PlayerInteractArgs args)
    {
      this.lastInteractable = interactable;
      ToolInteractArgs result = interactable.Interact(
          this.GetInteractArgs());
      this.OnTriggerInteraction = result.OnTrigger;
      if (result.ReceivedItem != null && 
        result.ReceivedItem.Data is TestMaterialItemData materialItemData) {
        this.HoldingItemData = materialItemData;
      }
      else if (result.IsMaterialItemTaken) {
        this.HoldingItemData = null;
      }
      if (result.DurationToPlayerStay != 0) {
        this.interactionDuration = result.DurationToPlayerStay;
        this.StartInteractionRoutine();
      }
      else {
        this.OnTriggerInteraction?.Invoke(this.lastInteractable);
      }
    }

    void StartInteractionRoutine()
    {
      this.toolInteractionRoutine = this.StartCoroutine(
        this.InteractionRoutine());   
    }

    IEnumerator InteractionRoutine()
    {
      this.meshRenderer.material.color = this.interactColor;
      yield return (new WaitForSeconds(this.interactionDuration));
      this.meshRenderer.material.color = this.normalColor;
      this.OnTriggerInteraction?.Invoke(this.lastInteractable);
    }

    #region Test code
    Rigidbody rb;
    [SerializeField] [Range(1f, 5f)]
    float movingSpeed;

    void Move(in Vector2 input)
    {
      this.rb.velocity = new Vector3(
        x: input.x,
        y: 0,
        z: input.y
        ) * this.movingSpeed; 
    }

    void Rotate(in Vector2 input)
    {
      this.transform.forward = new Vector3(
        x: input.x,
        y: 0,
        z: input.y);
    }

    Vector2 GetInput()
    {
      return (
        new Vector2(
          x: Input.GetAxis("Horizontal"),
          y: Input.GetAxis("Vertical")
          ).normalized);
    }

    void Awake()
    {
      this.rb = this.GetComponent<Rigidbody>();
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }
    #endregion
  }
}
