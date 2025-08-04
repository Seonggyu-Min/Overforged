using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EditorAttributes;

namespace SHG
{
  using ItemBox = SCR.BoxComponent;

  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyBotController : MonoBehaviour, IBot
  {
    public enum Leaf {
      MoveLeaf,
      GetMaterial,
      GiveItem,
      GetItem,
      Work,
      PickUpTong,
      RepeatWork
    }

    [SerializeField]
    Part partToCreate;

    public Item HoldingItem { 
      get => this.holdingItem;
      private set => this.holdingItem = value;
    }
    public int NetworkId => this.networkId;
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Transform Transform => this.transform;
    [SerializeField] [Required]
    Transform hands;
    [SerializeField] [Required]
    ItemBox box;
    [SerializeField] [Required]
    FurnaceComponent furnace;
    [SerializeField] [Required]
    AnvilComponent anvil;
    [SerializeField] [Required]
    TableComponent table;
    [SerializeField] [Required]
    QuenchingComponent quenchingTool;
    [SerializeField]
    RawMaterialBox[] materialBoxes;
    [SerializeField]
    EnemyBotBt behaviourTree;
    [SerializeField] [ReadOnly]
    BtNode.NodeState currentState;
    [SerializeField] [ReadOnly]
    Item holdingItem;
    [SerializeField]
    int networkId;
    float remainingDelay;
    Action workTrigger;
    public bool IsHoldingTong => this.tong != null;
    Transform tong;
    Transform[] allTongs = new Transform[0];
    BtNode[] allLeaves;
    SmithingToolComponent targetTool;
    public Transform[] GetTongs() => this.allTongs;

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
      item.transform.SetParent(this.hands);
      item.transform.position = this.hands.position;
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

    void Awake()
    {
      this.NavMeshAgent = this.GetComponent<NavMeshAgent>();
      this.CreateLeaves();
      this.behaviourTree = new EnemyBotBt();
    }

    void CreateLeaves()
    {
      this.allLeaves = new BtLeaf[System.Enum.GetValues(typeof(Leaf)).Length];
      this.allLeaves[(int)Leaf.MoveLeaf] = new BtMoveLeaf(
        target: Vector3.zero,
        bot: this,
        dist: 1f);
      this.allLeaves[(int)Leaf.GetMaterial] = new BtBringMaterialLeaf(
        box: null,
        bot: this);
      this.allLeaves[(int)Leaf.GiveItem] = new BtTransferItemLeaf(
        toGive: true,
        tool: null,
        transform: null,
        bot: this
        );
      this.allLeaves[(int)Leaf.GetItem] = new BtTransferItemLeaf(
        toGive: false,
        tool: null,
        transform: null,
        bot: this
        );
      this.allLeaves[(int)Leaf.Work] = new BtWorkLeaf(
        tool: null,
        transform: null,
        bot: this);
      this.allLeaves[(int)Leaf.PickUpTong] = new BtPickUpTongLeaf(bot: this
      );
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
          this.LooseItem();
        }
        var result = tool.Transfer(args);
        if (result.ReceivedItem != null) {
          this.GrabItem(result.ReceivedItem);
        }
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
      var result = tool.Work();
      Debug.Log($"work {tool} result: {result}");
      this.workTrigger = result.Trigger;
      this.Invoke(nameof(TriggerWork), 1f);
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
      }
    }

    public void PickUpTong(Transform tong)
    {
      if (!this.IsHoldingTong) {
        this.tong = tong;
        this.tong.SetParent(this.hands);
        this.tong.position = this.hands.position;
        var collider = this.tong.GetComponent<Collider>();
        if (collider != null) {
          collider.isTrigger = true;
        }
      }
    }

    void TriggerWork()
    {
      this.workTrigger?.Invoke();
    }

    void LooseItem()
    {
      Debug.Log($"loose item : {this.HoldingItem}");
      this.HoldingItem = null;
    }

    T GetLeaf<T>(Leaf leafType) where T: BtLeaf
    {
      return (this.allLeaves[(int)leafType] as T);
    }

    void Start()
    {
      this.allTongs = Array.ConvertAll(
        GameObject.FindGameObjectsWithTag("Tongs"),
        gameObject => gameObject.transform);
      var pickTong = this.GetLeaf<BtPickUpTongLeaf>(Leaf.PickUpTong);
      pickTong.Init();
    }

    [Button]
    void CreatePart()
    {
      this.behaviourTree = new EnemyBotBt(
        children: new BtNode[] { new BtCreatePartNode(
          part: this.partToCreate,
          bot: this
          ) }
        );
    }

    // Update is called once per frame
    void Update()
    {
      if (this.remainingDelay < 0) {
        this.currentState = this.behaviourTree.Evaluate();
      }
      else {
        this.remainingDelay -= Time.deltaTime;
      }
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
      return (tool != null);
    }

    public bool TryFindBox(RawMaterial rawMaterial, out ItemBox box)
    {
      int found = Array.FindIndex(
        this.materialBoxes,
        (materialBox) => rawMaterial.Equals(materialBox.Material));
      if (found != -1) {
        box = this.materialBoxes[found].Box;
        return (true);
      }      
      #if UNITY_EDITOR
        Debug.LogError($"{nameof(TryFindBox)}: Fail to Find box for {rawMaterial.MaterialType} {rawMaterial.OreType} {rawMaterial.WoodType}");
      #endif
      box = null;
      return (false);
    }
  }
}
