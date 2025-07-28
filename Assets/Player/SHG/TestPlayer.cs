using System;
using System.Collections;
using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;

namespace SHG
{
  [RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
  public class TestPlayer : MonoBehaviour
  {

    [SerializeField] [Range(1f, 3f)]
    float interactRadius;
    [SerializeField] [Range(1f, 10f)]
    float interactRange; 
    [SerializeField] [Range(1f, 5f)]
    float movingSpeed;
    [SerializeField, ReadOnly, HideInInspector]
    Item HoldingItem;
    public Action OnTriggerInteraction;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;
    [SerializeField]
    Transform hand;
    [SerializeField, TabGroup(nameof(HoldingItem), nameof(itemToCreate), nameof(itemPrefab))]
    Void itemGroup;
    Coroutine toolInteractionRoutine;
    MeshRenderer meshRenderer;
    [SerializeField, HideInInspector]
    ItemData itemToCreate;
    [SerializeField, HideInInspector]
    GameObject itemPrefab;
    float interactionDuration;

    [Button]
    void CreateItem()
    {
      GameObject itemObjct = GameObject.Instantiate(this.itemPrefab); 
      MaterialItem item = itemObjct.GetComponent<MaterialItem>();
      item.Data = this.itemToCreate;
      item.Ore = OreType.Gold;
      this.GrabItem(item);
    }

    void GrabItem(Item item)
    {
      Debug.Log($"GrabItem {item}");
      this.HoldingItem = item;
      item.transform.SetParent(this.hand);
      item.transform.localPosition = Vector3.zero;
    }

    bool IsTryingInteract()
    {
      return (Input.GetKeyDown(KeyCode.E));
    }

    bool IsTryingGrab()
    {
      return (Input.GetKeyDown(KeyCode.G));
    }

    public void LooseItem()
    {
      if (this.HoldingItem != null) {
        this.HoldingItem = null;
      }
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
        ToolTransferArgs args;
        if (this.HoldingItem is MaterialItem materialItem) {
          args = new ToolTransferArgs {
            ItemToGive = materialItem,
            PlayerNetworkId = 1
          };
        }
        else {
          args = new ToolTransferArgs { 
            ItemToGive = null,
            PlayerNetworkId = 1 
          };
        }
        if (transferTool.CanTransferItem(args)) { 
          this.TransferItem(transferTool, args);
        }
      }
    }

    void TransferItem(IInteractableTool tool, in ToolTransferArgs args)
    {
      ToolTransferResult result = tool.Transfer(args);
      this.LooseItem();
      if (result.ReceivedItem != null) {
        this.GrabItem(result.ReceivedItem);
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
