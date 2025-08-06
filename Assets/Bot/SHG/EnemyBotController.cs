using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EditorAttributes;

namespace SHG
{
  using ItemBox = SCR.BoxComponent;
  using ConveyComponent = NewProductConveyComponent;

  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyBotController : MonoBehaviour, IBot
  {

    [SerializeField]
    Part partToCreate;
    [SerializeField]
    ProductRecipe productToCreate;

    public Item HoldingItem { 
      get => this.holdingItem;
      private set => this.holdingItem = value;
    }
    public int NetworkId {
      get => this.networkId;
      set => this.networkId = value;
    }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Transform Transform => this.transform;
    public bool IsHoldingTong => this.tong != null;
    public Transform[] GetTongs() => this.allTongs;
    public Action<IInteractableTool> OnWork { get; set; }
    public Action OnFinishWork { get; set; }
    public Action<Item> OnRoot { get; set; }
    public bool IsOwner;
    [SerializeField] [Required]
    Transform rightHand;
    [SerializeField] [Required]
    Transform leftHand;
    [SerializeField] [ReadOnly]
    FurnaceComponent furnace;
    [SerializeField] [ReadOnly]
    AnvilComponent anvil;
    [SerializeField] [ReadOnly]
    TableComponent table;
    [SerializeField] [ReadOnly]
    QuenchingComponent quenchingTool;
    [SerializeField]
    EnemyBotBt behaviourTree;
    BtNode.NodeState currentState;
    [SerializeField] [ReadOnly]
    Item holdingItem;
    [SerializeField]
    int networkId;
    float remainingDelay;
    Action workTrigger;
    Transform tong;
    Transform[] allTongs = new Transform[0];
    BtNode[] allLeaves;
    SmithingToolComponent targetTool;

    public void GrabItem(Item item)
    {
      #if UNITY_EDITOR
      if (this.HoldingItem != null) {
        Debug.LogError($"{nameof(EnemyBotController)}: {nameof(HoldingItem)} is not null");
        return;
      }
      #endif
      if (item.transform.parent != null &&
        item.transform.parent.tag == "Player") {
      #if UNITY_EDITOR
        Debug.LogError($"{nameof(EnemyBotController)}: {item} is holding by other player");
      #endif
        return;
      }
      this.OnRoot?.Invoke(item);
      Debug.Log($"GrabItem: {item}");
      this.HoldingItem = item;
      var rigidbody = item.GetComponent<Rigidbody>();
      if (rigidbody != null) {
        rigidbody.useGravity = false;
      }
      var collider = item.GetComponent<Collider>();
      if (collider != null) {
        collider.isTrigger = true;
      }
      if (item is MaterialItem materialItem && materialItem.IsHot) {
        materialItem.transform.SetParent(this.leftHand);
        materialItem.transform.position = this.leftHand.position + this.leftHand.forward;
      }
      else {
        item.transform.SetParent(this.rightHand);
        item.transform.position = this.rightHand.position;
      }
      this.remainingDelay = Math.Max(this.remainingDelay + 0.5f, 0.5f);
    }

    public void WaitForSeconds(float second)
    {
      if (this.remainingDelay > 0) {
        this.remainingDelay += second;
      }
      else {
        this.remainingDelay = second;
      }
    }

    BtNode GetItemNode(
      IInteractableTool tool,
      Transform transform, BtNode parent = null)
    {
      BtNode  getItem = new BtTransferItemLeaf(
        toGive: false,
        tool: tool,
        transform: transform,
        bot: this
        );
      BtNode pickUp = new BtPickUpTongLeaf(bot: this);
      BtNode pickUpAndGetItem = new BtSequenceNode(children: new BtNode[] { pickUp, getItem });
      return ( new BtConditionalNode(
        condition: () => (this.targetTool != null &&
        this.targetTool.HoldingItem.IsHot),
        trueNode: pickUpAndGetItem,
        falseNode: pickUp
        ));
    }

    public bool TryTransferItem(IInteractableTool tool)
    {
      var args = new ToolTransferArgs {
          ItemToGive = this.HoldingItem,
          PlayerNetworkId = this.NetworkId
        };
      if (tool.CanTransferItem(new ToolTransferArgs {
          ItemToGive = this.HoldingItem,
          PlayerNetworkId = this.NetworkId
        })) {
        if (args.ItemToGive != null) {
          this.PutDownItem();
        }
        var result = tool.Transfer(args);
        if (result.ReceivedItem != null) {
          this.GrabItem(result.ReceivedItem);
        }
        Debug.LogWarning($"Transfer: {tool}");
        return (true);
      }
      else {
        #if UNITY_EDITOR
        Debug.LogError($"Fail to transfer Item {args}");
        #endif
        return (false);
      }
    }
    
    public ToolWorkResult Work(IInteractableTool tool)
    {
      Debug.LogWarning($"work: {tool}");
      var result = tool.Work();
      Debug.Log($"work {tool} result: {result}");
      this.workTrigger = result.Trigger;
      this.OnWork?.Invoke(tool);
      this.Invoke(nameof(TriggerWork), result.DurationToStay);
      return (result);
    }

    public void PutDownTong()
    {
      if (this.IsHoldingTong) {
        this.tong.SetParent(null);
        var collider = this.tong.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = false;
        }
        var rb = this.tong.GetComponent<Rigidbody>();
        if (rb != null) {
          rb.isKinematic = false;
        }
      }
    }

    public void PickUpTong(Transform tong)
    {
      if (!this.IsHoldingTong) {
        this.tong = tong;
        this.tong.SetParent(this.leftHand);
        this.tong.position = this.leftHand.position;
        this.tong.transform.localRotation = Quaternion.identity;
        var collider = this.tong.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = true;
        }
        var rb = this.tong.GetComponent<Rigidbody>();
        if (rb != null) {
          rb.isKinematic = true;
        }
      }
    }

    public T GetLeaf<T>(BtLeaf.Type leafType) where T: BtLeaf
    {
      return (this.allLeaves[(int)leafType] as T);
    }

    public bool IsHoldingHotMaterial()
    {
      if (this.HoldingItem != null &&
        this.HoldingItem is MaterialItem materialItem) {
        return (materialItem.IsHot);
      }
      return (false);
    }

    public bool TryFindTool(SmithingTool.ToolType toolType, out SmithingToolComponent tool)
    {
      tool = null;
      switch (toolType) {
        case (SmithingTool.ToolType.Furnace):
        tool = this.furnace;
        break;
        case (SmithingTool.ToolType.Anvil):
        tool = this.anvil;
        break;
        case (SmithingTool.ToolType.QuenchingTool):
        tool = this.quenchingTool;
        break;
        case (SmithingTool.ToolType.WoodTable):
        case (SmithingTool.ToolType.CraftTable):
        tool = this.table;
        break;
      }
      #if UNITY_EDITOR
      if (tool == null) {
        Debug.LogError($"{nameof(TryFindTool)}: Fail to Find {toolType}");
      }
      #endif
      return (tool != null);
    }

    public bool TryFindBox(RawMaterial rawMaterial, out ItemBox box)
    {
      int found = Array.FindIndex(
        BotContext.Instance.MaterialBoxes,
        (materialBox) => rawMaterial.Equals(materialBox.Material));
      if (found != -1) {
        box = BotContext.Instance.MaterialBoxes[found].Box;
        return (true);
      }      
      #if UNITY_EDITOR
        Debug.LogError($"{nameof(TryFindBox)}: Fail to Find box for {rawMaterial.MaterialType} {rawMaterial.OreType} {rawMaterial.WoodType}");
      #endif
      box = null;
      return (false);
    }

    public bool TryGetSubmitPlace(out ConveyComponent submitPlace)
    {
      return (BotContext.Instance.TryGetClosestSubmitPlace(
          this.transform.position, out submitPlace));
    }

    void Awake()
    {
      this.NavMeshAgent = this.GetComponent<NavMeshAgent>();
      this.CreateLeaves();
      this.behaviourTree = new EnemyBotBt(this);
    }

    void Start()
    {
      this.allTongs = Array.ConvertAll(
        GameObject.FindGameObjectsWithTag("Tongs"),
        gameObject => gameObject.transform);
      var pickTong = this.GetLeaf<BtPickUpTongLeaf>(BtLeaf.Type.PickUpTong);
      pickTong.Init();
      this.GetContext();
    }

    // Update is called once per frame
    void Update()
    {
      if (this.IsOwner && this.remainingDelay < 0) {
        this.currentState = this.behaviourTree.Evaluate();
      }
      else {
        this.remainingDelay -= Time.deltaTime;
      }
    }


    void TriggerWork()
    {
      this.workTrigger?.Invoke();
      this.OnFinishWork?.Invoke();
    }

    public void PutDownItem()
    {
      Debug.Log($"loose item : {this.HoldingItem}");
      if (this.HoldingItem != null) {
        this.HoldingItem.transform.SetParent(null);
        this.HoldingItem = null;
      }
    }

    void GetContext()
    {
      this.anvil = BotContext.Instance.GetComponent<AnvilComponent>(
        this.networkId, SmithingTool.ToolType.Anvil);
      this.furnace = BotContext.Instance.GetComponent<FurnaceComponent>(
        this.networkId, SmithingTool.ToolType.Furnace);
      this.quenchingTool = BotContext.Instance.GetComponent<QuenchingComponent>(
        this.networkId, SmithingTool.ToolType.QuenchingTool);
      this.table = BotContext.Instance.GetComponent<TableComponent>(
        this.networkId, SmithingTool.ToolType.WoodTable);
    }

    [Button]
    void CreatePart()
    {
      this.behaviourTree = new EnemyBotBt(
        bot: this,
        children: new BtNode[] { 
        new BtCreatePartNode(
          part: this.partToCreate,
          bot: this),
        new BtConditionalNode(
          condition: () => this.IsHoldingHotMaterial(),
          trueNode: new BtQuenchingNode(bot: this),
          falseNode: null)
        });
    }

    [Button]
    public void StartCreateProduct()
    {
      this.behaviourTree = EnemyBotBt.KeepCraftingProductBt(this);
    }

    void CreateLeaves()
    {
      this.allLeaves = new BtLeaf[System.Enum.GetValues(
        typeof(BtLeaf.Type)).Length];
      this.allLeaves[(int)BtLeaf.Type.MoveLeaf] = new BtMoveLeaf(
        target: Vector3.zero,
        bot: this,
        dist: 1f);
      this.allLeaves[(int)BtLeaf.Type.GetMaterial] = new BtBringMaterialLeaf(
        box: null,
        bot: this);
      this.allLeaves[(int)BtLeaf.Type.GiveItem] = new BtTransferItemLeaf(
        toGive: true,
        tool: null,
        transform: null,
        bot: this
        );
      this.allLeaves[(int)BtLeaf.Type.GetItem] = new BtTransferItemLeaf(
        toGive: false,
        tool: null,
        transform: null,
        bot: this
        );
      this.allLeaves[(int)BtLeaf.Type.Work] = new BtWorkLeaf(
        tool: null,
        transform: null,
        bot: this);
      this.allLeaves[(int)BtLeaf.Type.PickUpTong] = new BtPickUpTongLeaf(bot: this

      );
    }

  }
}
