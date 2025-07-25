using System;
using System.Collections;
using UnityEngine;
using EditorAttributes;
using Unity.VisualScripting;
using UnityEditor;

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
    [SerializeField]
    TestItemData HodlingProductData;
    public Action OnTriggerInteraction;
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

    bool TryFindInteratable(out IInteractableTool interactable)
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
      interactable = hitInfo.collider.GetComponent<IInteractableTool>();
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
        this.TryFindInteratable(out IInteractableTool workTool) &&
        workTool.CanWork())
      {
        this.Work(workTool);
      }
      else if (this.IsTryingGrab() &&
        this.TryFindInteratable(out IInteractableTool transferTool))
      {
        ToolTransferArgs args = new ToolTransferArgs
        {
          ItemToGive = this.HoldingItem,
          PlayerNetworkId = 1
        };
        if (transferTool.CanTransferItem(args)) { 
          this.TransferItem(transferTool, args);
        }
      }
    }

    void TransferItem(IInteractableTool tool, in ToolTransferArgs args)
    {
      if (this.HoldingItem != null)
      {
        this.HoldingItemData = null;
      }
      ToolTransferResult result = tool.Transfer(args);
      if (result.ReceivedItem != null) {
        if (result.ReceivedItem is TestMaterialItem materialItem)
        {
          this.HoldingItemData = (TestMaterialItemData)materialItem.Data;

        }
        else
        {
          this.HodlingProductData = result.ReceivedItem.Data;
        }
      }
    }

    void Work(in IInteractableTool tool)
    {
      this.OnTriggerInteraction = null;
      ToolWorkResult result = tool.Work();
      if (result.DurationToStay != 0) {
        this.StartInteractionRoutine(result);
      }
      else {
        result.Trigger?.Invoke();
      }
    }

    void StartInteractionRoutine(ToolWorkResult work)
    {
      this.interactionDuration = work.DurationToStay;
      this.OnTriggerInteraction = work.Trigger;
      this.toolInteractionRoutine = this.StartCoroutine(
        this.InteractionRoutine());   
    }

    IEnumerator InteractionRoutine()
    {
      this.meshRenderer.material.color = this.interactColor;
      yield return (new WaitForSeconds(this.interactionDuration));
      this.meshRenderer.material.color = this.normalColor;
      this.OnTriggerInteraction?.Invoke();
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
