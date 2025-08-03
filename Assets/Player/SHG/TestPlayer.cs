#define LOCAL_TEST
using System;
using System.Collections;
using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;
using Photon.Pun;

namespace SHG
{
  [RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(PhotonView))]
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
    [SerializeField, TabGroup(nameof(HoldingItem), nameof(itemToCreate), nameof(itemPrefab), nameof(itemPrefabPath))]
    Void itemGroup;
    Coroutine toolInteractionRoutine;
    MeshRenderer meshRenderer;
    [SerializeField, HideInInspector]
    ItemData itemToCreate;
    [SerializeField, HideInInspector]
    GameObject itemPrefab;
    [SerializeField, HideInInspector]
    string itemPrefabPath;
    float interactionDuration;
    PhotonView photonView;
    Vector3 raycastOffset = new Vector3(0, 0.3f, 0);

    [Button] [PunRPC]
    void CreateItem()
    {
      if (this.itemToCreate == null) {
        Debug.LogError("no item to create");
        return ;
      }
      if (this.photonView.IsMine) {
        GameObject itemObject;
        #if LOCAL_TEST
        itemObject = GameObject.Instantiate(this.itemPrefab); 
        #else   
        itemObject = PhotonNetwork.Instantiate(this.itemPrefabPath, Vector3.zero, Quaternion.identity);
        #endif
        MaterialItem item = itemObject.GetComponent<MaterialItem>();
        item.Data = this.itemToCreate;
        item.Ore = OreType.Gold;
        this.GrabItem(item);
        #if !LOCAL_TEST
        this.photonView.RPC(
          methodName: nameof(CreateItem),
          target: RpcTarget.OthersBuffered);
        #endif
      }
      else if (this.HoldingItem != null) {
        
        this.HoldingItem.Data = this.itemToCreate;
        if (this.HoldingItem is MaterialItem materialItem) {
          materialItem.Ore = OreType.Gold;
        }
      }
    }

    [Button]
    void DestroyItem()
    {
      if (this.HoldingItem == null) {
        Debug.LogError("no item to destroy");
        return ;
      }
      PhotonNetwork.Destroy(this.HoldingItem.gameObject);
      //Destroy(this.HoldingItem.gameObject);
      this.HoldingItem = null;
    }

    [Button]
    void ChangeCurrentItem()
    {
      if (this.HoldingItem == null) {
        Debug.LogError("no item to change");
        return; 
      } 
      this.HoldingItem.Data = this.itemToCreate;
      if (this.HoldingItem is MaterialItem materialItem) {
        materialItem.Ore = OreType.Gold;
      }
    }

    public void GrabItem(Item item)
    {
      Debug.Log($"GrabItem {item}");
      this.HoldingItem = item;
      this.HoldingItem.GetComponent<Rigidbody>().isKinematic = true;
      item.transform.SetParent(this.hand);
      item.transform.localPosition = Vector3.zero;
      #if !LOCAL_TEST
      if (this.photonView.IsMine) {
        this.photonView.RPC(
          methodName: nameof(GrabItemNetwork),
          target: RpcTarget.Others,
          parameters: new object[] {
          item.photonView.ViewID
          });
      }
      #endif
    }

    [PunRPC]
    public void GrabItemNetwork(int itemId)
    {
      Debug.Log($"GrabItemNetwork {itemId}");
      PhotonView photonView = PhotonView.Find(itemId);
      if (photonView != null) {
        Item item = photonView.GetComponent<Item>(); 
        Debug.Log($"GrabItemNetwork {item}");
        if (item != null) {
          this.GrabItem(item);
        }
      }
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

    bool TryFindItem(out Item item)
    {
      #if UNITY_EDITOR
      Debug.DrawLine(
        start: this.transform.position + this.raycastOffset,
        end: this.transform.position + this.raycastOffset + this.transform.forward * this.interactRange,
        color: Color.blue,
        duration: 0.5f);
      #endif
      bool isHit = Physics.SphereCast(
        origin: this.transform.position + this.raycastOffset,
        radius: this.interactRadius,
        direction: this.transform.forward,
        hitInfo: out RaycastHit hitInfo,
        maxDistance: this.interactRange);
      if (!isHit) {
        item = null;
        return (false);
      } 
      item = hitInfo.collider.GetComponent<Item>();
      return (item!= null);
    }

    bool TryFindInteratable(out IInteractableTool interactable)
    {
#if UNITY_EDITOR
      Debug.DrawLine(
        start: this.transform.position + this.raycastOffset,
        end: this.transform.position + this.raycastOffset + this.transform.forward * this.interactRange,
        color: Color.blue,
        duration: 0.5f);
#endif
      bool isHit = Physics.SphereCast(
        origin: this.transform.position + this.raycastOffset,
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
      if (!this.photonView.IsMine) {
        return ; 
      }
      var movingInput = this.GetInput();
      if (movingInput != Vector2.zero) {
        this.Move(movingInput);
        this.Rotate(movingInput);
      }
      else {
        this.rb.velocity = Vector2.zero;
      }
      if (this.IsTryingGrab() &&
        this.TryFindItem(out Item item)) {
        this.GrabItem(item);
        return ;
      }
      if (!this.TryFindInteratable(out IInteractableTool interactable)) {
        return ;
      }
      ToolTransferArgs transferArgs;
      if (this.HoldingItem != null && 
        this.HoldingItem is MaterialItem materialItem) {
        transferArgs = new ToolTransferArgs {
          ItemToGive = materialItem,
          PlayerNetworkId = 1
        };
      }
      else {
        transferArgs = new ToolTransferArgs { 
          ItemToGive = null,
          PlayerNetworkId = 1 
        };
      }
      bool canTransfer = interactable.CanTransferItem(transferArgs);

      if (interactable is SmithingToolComponent smithingTool) {
        if (smithingTool.CanWork()) {
          smithingTool.HighlightInstantly(Color.green); 
        }
        else if (canTransfer) {
          smithingTool.HighlightInstantly(Color.blue); 
        }
        else {
          smithingTool.HighlightInstantly(Color.yellow);
        }
      }

      if (this.IsTryingInteract() &&
        interactable.CanWork()) {
        this.Work(interactable);
      }
      else if (this.IsTryingGrab() && canTransfer) {
        this.TransferItem(interactable, transferArgs);
      }
    }

    void TransferItem(IInteractableTool tool, in ToolTransferArgs args)
    {
      if (this.HoldingItem != null) {
        this.HoldingItem.GetComponent<Rigidbody>().isKinematic = false;
      }
      ToolTransferResult result = tool.Transfer(args);
      this.LooseItem();
      if (result.ReceivedItem != null) {
        Debug.Log($"ReceivedItem: {result.ReceivedItem}");
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
      this.photonView = this.GetComponent<PhotonView>();
    }

    void Start()
    {
      var cameraController = CameraController.Instance;;
      Debug.Log($"cameraController : {cameraController}");
      cameraController.Player = this.transform;
      cameraController.gameObject.SetActive(true);
    }
    #endregion
  }
}
