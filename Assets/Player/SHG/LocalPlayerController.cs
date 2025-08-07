using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using EditorAttributes;
using Void = EditorAttributes.Void;
using Unity.VisualScripting;
using SCR;
using UnityEngine.UI;
//using Photon.Pun;

namespace SHG
{
  //[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(PhotonView))]
  public class LocalPlayerController : MonoBehaviour
  {
    const float CHANGE_MODE_DELAY = 0.5f;
    const float DASH_DELAY = 1f;
    const float DASH_DURATION = 0.3f;
    enum Mode
    {
      Normal,
      Work
    }
    SingletonAudio audioPlayer;
    public Action OnTriggerInteraction;
    [SerializeField]
    GameObject indicator;
    [SerializeField] [Range(0.1f, 3f)]
    float interactRadius;
    [SerializeField] [Range(1f, 10f)]
    float interactRange; 
    [SerializeField] [Range(1f, 5f)]
    float movingSpeed;
    [SerializeField, ReadOnly, HideInInspector]
    Item HoldingItem;
    [SerializeField] [Required]
    Transform hand;
    [SerializeField] [Required]
    Transform itemPos;
    [SerializeField] [Required]
    Transform tongPos;
    [SerializeField] [Required]
    Transform hammer;
    [SerializeField] [Required]
    Animator animator;
    [SerializeField]
    float dashForce;
    [SerializeField]
    float dashAcceleration;
    float interactionDuration;
    Vector3 raycastOffset = new Vector3(0, 0.3f, 0);
    Coroutine toolInteractionRoutine;
    Rigidbody rb;
    Vector2 movingInput;
    [SerializeField] [ReadOnly]
    Mode mode = Mode.Normal;
    float delay;
    Coroutine currentRoutine;
    public bool IsAbleToMove = true;
    bool isTryInteracting = false;
    bool isTryToThrow = false;
    SfxController walkSfx;
    bool isPlayingWalkSfx;
    Transform tong;
    int itemLayer;
    int toolLayer;
    //    [SerializeField]
    //    Color normalColor;
    //    [SerializeField]
    //    Color interactColor;
    //    [SerializeField, TabGroup(nameof(HoldingItem), nameof(itemToCreate), nameof(itemPrefab), nameof(itemPrefabPath))]
    //    Void itemGroup;
    //    MeshRenderer meshRenderer;
    //    [SerializeField, HideInInspector]
    //    ItemData itemToCreate;
    //    [SerializeField, HideInInspector]
    //    GameObject itemPrefab;
    //    [SerializeField, HideInInspector]
    //    string itemPrefabPath;
    //    PhotonView photonView;

    public void GrabItem(Item item)
    {
      this.HoldingItem = item;
      var rigidBody = this.HoldingItem.GetComponent<Rigidbody>();
      if (rigidBody != null) {
        rigidBody.isKinematic = true;
      }
      var collider = this.HoldingItem.GetComponent<Collider>();
      if (collider != null) {
        collider.isTrigger = true;
      }
      item.transform.SetParent(this.transform);
      item.transform.position = this.itemPos.position;
      this.animator.SetBool("Hold", true);
      this.audioPlayer.PlayRandomSfx("grab");
      this.isTryInteracting = false;
      //      #if !LOCAL_TEST
      //      if (this.photonView.IsMine) {
      //        this.photonView.RPC(
      //          methodName: nameof(GrabItemNetwork),
      //          target: RpcTarget.Others,
      //          parameters: new object[] {
      //          item.photonView.ViewID
      //          });
      //      }
      //      #endif
    }

    public void LooseItem()
    {
      if (this.HoldingItem != null) {
        this.HoldingItem.transform.SetParent(null);
        var rigidBody = this.HoldingItem.GetComponent<Rigidbody>();
        if (rigidBody != null) {
          rigidBody.isKinematic = false;
        }
        var collider = this.HoldingItem.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = false;
        }
        this.animator.SetBool("Hold", false);
        this.audioPlayer.PlayRandomSfx("playerThrow");
        this.HoldingItem = null;
      }
      this.isTryToThrow = false;
    }

    void GrabTong(Transform tong)
    {
      if (!this.tong) {
        this.tong = tong;
        this.tong.SetParent(this.tongPos);
        this.tong.position = this.tongPos.position;
        this.tong.transform.localRotation = Quaternion.identity;
        var collider = this.tong.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = true;
        }
        var rb = this.tong.GetComponent<Rigidbody>();
        if (rb != null) {
          rb.isKinematic = true;
        }
        this.audioPlayer.PlayRandomSfx("grab");
      }
      this.isTryInteracting = false;
    }

    void LooseTong()
    {
      if (this.tong != null) {
        this.tong.SetParent(null);
        var collider = this.tong.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = false;
        }
        var rb = this.tong.GetComponent<Rigidbody>();
        if (rb != null) {
          rb.isKinematic = false;
        }
        this.tong = null;
        this.audioPlayer.PlayRandomSfx("grab");
      }
      this.isTryToThrow = false;
    }

    bool TryFind<T>(out T found, in string tag = null) where T: Component
    {
      Collider[] hitColliders = Physics.OverlapSphere(
        position: this.transform.position,
        radius: this.interactRange,
        layerMask: this.itemLayer
        );
      foreach (var collider in hitColliders) {
        if (tag != null && collider.tag != tag) {
          continue;
        }
        var component = collider.gameObject.GetComponent<T>();
        if (component != null) {
          var dir = collider.transform.position - this.transform.position;
          var dot = Vector3.Dot(dir, this.transform.forward);
          dot = Mathf.Clamp(dot, -1f, 1f);
          var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
          if (angle < 180f) {
            found = component;
            return (true);
          }
        }
      }
      found = null;
      return (false);
    }

    bool TryFindItemBox(out BoxComponent itemBox)
    {
      bool isHit = Physics.SphereCast(
        origin: this.transform.position + this.raycastOffset,
        radius: this.interactRadius,
        direction: this.transform.forward,
        hitInfo: out RaycastHit hitInfo,
        layerMask: this.toolLayer,
        maxDistance: this.interactRange);
      if (!isHit) {
        itemBox = null;
        return (false);
      }
      itemBox = hitInfo.collider.GetComponent<BoxComponent>();
      return (itemBox != null);
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
        layerMask: this.toolLayer,
        maxDistance: this.interactRange);
      if (!isHit) {
        interactable = null;
        return (false);
      } 
      interactable = hitInfo.collider.GetComponent<IInteractableTool>();
      return (interactable != null);
    }

    void OnChangeState()
    {
      if (this.delay <= 0) {
        this.mode = this.mode == Mode.Work ? Mode.Normal : Mode.Work;
        this.animator.SetBool("Work", this.mode == Mode.Work);
        this.animator.SetTrigger("ChangeState");
        this.audioPlayer.PlayRandomSfx("playerChangeState");
        this.delay = CHANGE_MODE_DELAY;
        this.hammer.gameObject.SetActive(this.mode == Mode.Work);
        if (this.tong != null) {
          this.tong.gameObject.SetActive(this.mode == Mode.Normal);
        }
      }
    }

    private void OnDash()
    {
      if (this.delay <= 0) {
        this.delay = DASH_DELAY;
        if (this.currentRoutine != null) {
          this.StopCoroutine(this.currentRoutine);
        }
        this.IsAbleToMove = false;
        this.animator.SetTrigger("Dash");
        this.currentRoutine = this.StartCoroutine(this.DashRoutine());
        this.audioPlayer.PlayRandomSfx("playerDash");
      }
    }

    IEnumerator DashRoutine()
    {
      float duration = 0f;
      this.rb.AddForce(
        this.transform.forward * this.dashForce,
        ForceMode.Impulse
        );
      while (duration < DASH_DURATION) {
        this.rb.AddForce(
          this.transform.forward * this.dashAcceleration,
          ForceMode.Acceleration);
        duration += Time.deltaTime;
        yield return (null);
      }
      this.IsAbleToMove = true;
    }

    private void OnAction()
    {
      this.isTryInteracting = true;
    }

    void FixedUpdate()
    {
      if (movingInput != Vector2.zero) {
        this.Move(movingInput);
        this.Rotate(movingInput);
        this.animator.SetBool("Walk", true);
        if (!this.isPlayingWalkSfx) {
          this.walkSfx.PlayBack();
          this.isPlayingWalkSfx = true;
        }
      }
      else {
        this.rb.velocity = Vector2.zero;
        this.animator.SetBool("Walk", false);
        if (this.isPlayingWalkSfx) {
          this.walkSfx.Pause();
          this.isPlayingWalkSfx = false;
        }
      }
    }

    void Update()
    {
      this.delay -= Time.deltaTime;
      //      if (!this.photonView.IsMine) {
      //        return ; 
      //      }
      if (this.HoldingItem != null && this.isTryToThrow) {
        this.LooseItem();
        return;
      }
      if (this.tong != null && this.isTryToThrow) {
        this.LooseTong();
        return;
      }
      if (this.tong == null &&
        this.isTryInteracting &&
        this.TryFind<Transform>(out Transform foundTransform, tag:
          "Tongs")) {
        this.GrabTong(foundTransform);
        return;
      }
      if (this.HoldingItem == null &&
        this.mode == Mode.Normal &&
        this.isTryInteracting &&
        this.TryFind<Item>(out Item item) &&
        item.transform.parent == null) {
        Debug.LogWarning("GrabItem");
        if (item is MaterialItem materialToGrab) {
          if ((item.IsHot && this.tong != null) ||
            (!item.IsHot && this.tong == null)) {
            this.GrabItem(item);
          }
        }
        else {
          this.GrabItem(item);
        }
        return;
      }
      if (this.isTryInteracting &&
        this.mode == Mode.Normal &&
        this.HoldingItem == null &&
        this.TryFindItemBox(out BoxComponent itemBox)) {
        this.GetItemFrom(itemBox);
        return;
      }
      if (!this.TryFindInteratable(out IInteractableTool interactable)) {
        this.isTryInteracting = false;
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
        if (smithingTool.HoldingItem != null &&
          smithingTool.HoldingItem.IsHot) {
          canTransfer = canTransfer && this.tong != null;
        }
        else if (smithingTool.HoldingItem != null) {
          canTransfer = canTransfer && this.tong == null;
        }
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
      else if (interactable is DoorController door) {
        if (this.HoldingItem != null && this.HoldingItem is ProductItem product) {
          door.HighlightInstantly(Color.green);
        }
        else {
          door.HighlightInstantly(Color.yellow);
        }
      }
      if (!this.isTryInteracting) {
        return;
      }
      if (this.mode == Mode.Work && interactable.CanWork())
      {
        this.Work(interactable);
      }
      else if (this.mode == Mode.Normal && canTransfer)
      {
        Debug.Log("Transfer");
        this.TransferItem(interactable, transferArgs);
      }
    }

    void GetItemFrom(BoxComponent itemBox)
    {
      var itemObject = itemBox.CreateItem();
      Item item = itemObject.GetComponent<Item>();
      if (item != null) {
        this.GrabItem(item);
      }
      this.isTryInteracting = false;
    }

    void TransferItem(IInteractableTool tool, in ToolTransferArgs args)
    {
      if (this.HoldingItem != null) {
        this.HoldingItem.GetComponent<Rigidbody>().isKinematic = false;
      }
      this.LooseItem();
      ToolTransferResult result = tool.Transfer(args);
      if (result.ReceivedItem != null) {
        this.GrabItem(result.ReceivedItem);
      }
      this.isTryInteracting = false;
    }

    void Work(in IInteractableTool tool)
    {
      this.OnTriggerInteraction = null;
      ToolWorkResult result = tool.Work();
      this.audioPlayer.PlayRandomSfx("playerHammering");
      if (result.DurationToStay != 0) {
        this.StartInteractionRoutine(result);
      }
      else {
        result.Trigger?.Invoke();
      }
      this.isTryInteracting = false;
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
      //     this.meshRenderer.material.color = this.interactColor;
      yield return (new WaitForSeconds(this.interactionDuration));
      //      this.meshRenderer.material.color = this.normalColor;
      this.OnTriggerInteraction?.Invoke();
    }

    void Move(in Vector2 input)
    {
      if (this.IsAbleToMove) {
        this.rb.velocity = new Vector3(
          x: input.x,
          y: 0,
          z: input.y
          ) * this.movingSpeed;
      }
    }

    void Rotate(in Vector2 input)
    {
      this.transform.forward = new Vector3(
        x: input.x,
        y: 0,
        z: input.y);
    }

    void OnMove(InputValue value)
    {
      this.movingInput = value.Get<Vector2>().normalized;
    }

    void OnThrow()
    {
      this.isTryToThrow = true;
    }

    void Awake()
    {
      this.itemLayer = (1 << LayerMask.NameToLayer("Item"));
      this.toolLayer = (1 << LayerMask.NameToLayer("InteractionObject"));
      this.rb = this.GetComponent<Rigidbody>();
      //      this.meshRenderer = this.GetComponent<MeshRenderer>();
      //      this.photonView = this.GetComponent<PhotonView>();
    }

    void Start()
    {
      this.audioPlayer = SingletonAudio.Instance;
      this.walkSfx = this.audioPlayer
        .PlayRandomSfx("playerWalk")
        .SetLoop(true)
        .SetVolume(0.5f)
        .Pause();
      this.indicator.gameObject.SetActive(true);
      var cameraController = CameraController.Instance;;
      Debug.Log($"cameraController : {cameraController}");
      cameraController.Player = this.transform;
      cameraController.gameObject.SetActive(true);
    }

    //    [Button] [PunRPC]
    //    void CreateItem()
    //    {
    //      if (this.itemToCreate == null) {
    //        Debug.LogError("no item to create");
    //        return ;
    //      }
    //      if (this.photonView.IsMine) {
    //        GameObject itemObject;
    //        #if LOCAL_TEST
    //        itemObject = GameObject.Instantiate(this.itemPrefab); 
    //        #else   
    //        itemObject = PhotonNetwork.Instantiate(this.itemPrefabPath, Vector3.zero, Quaternion.identity);
    //        #endif
    //        MaterialItem item = itemObject.GetComponent<MaterialItem>();
    //        item.Data = this.itemToCreate;
    //        item.Ore = OreType.Gold;
    //        this.GrabItem(item);
    //        #if !LOCAL_TEST
    //        this.photonView.RPC(
    //          methodName: nameof(CreateItem),
    //          target: RpcTarget.OthersBuffered);
    //        #endif
    //      }
    //      else if (this.HoldingItem != null) {
    //        
    //        this.HoldingItem.Data = this.itemToCreate;
    //        if (this.HoldingItem is MaterialItem materialItem) {
    //          materialItem.Ore = OreType.Gold;
    //        }
    //      }
    //    }
    //
    //    [Button]
    //    void DestroyItem()
    //    {
    //      if (this.HoldingItem == null) {
    //        Debug.LogError("no item to destroy");
    //        return ;
    //      }
    //      PhotonNetwork.Destroy(this.HoldingItem.gameObject);
    //      //Destroy(this.HoldingItem.gameObject);
    //      this.HoldingItem = null;
    //    }
    //
    //    [Button]
    //    void ChangeCurrentItem()
    //    {
    //      if (this.HoldingItem == null) {
    //        Debug.LogError("no item to change");
    //        return; 
    //      } 
    //      this.HoldingItem.Data = this.itemToCreate;
    //      if (this.HoldingItem is MaterialItem materialItem) {
    //        materialItem.Ore = OreType.Gold;
    //      }
    //    }

    //    [PunRPC]
    //    public void GrabItemNetwork(int itemId)
    //    {
    //      Debug.Log($"GrabItemNetwork {itemId}");
    //      PhotonView photonView = PhotonView.Find(itemId);
    //      if (photonView != null) {
    //        Item item = photonView.GetComponent<Item>(); 
    //        Debug.Log($"GrabItemNetwork {item}");
    //        if (item != null) {
    //          this.GrabItem(item);
    //        }
    //      }
    //    }

  }
}
