using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using TMPro;
using Void = EditorAttributes.Void;
using Zenject;

namespace SHG
{
  [RequireComponent(typeof(MeshRenderer), typeof(Animator))]
  public class TableComponent : SmithingToolComponent
  {
    [Inject]
    IAudioLibrary audioLibrary;
    [SerializeField]
    [ReadOnly]
    WoodTable woodTable;
    [SerializeField]
    SmithingToolData woodTableData;
    [SerializeField]
    [ReadOnly]
    CraftTable craftTable;
    [SerializeField]
    CraftTableData craftTableData;
    [SerializeField]
    [Required()]
    Transform materialPosition;

    [SerializeField]
    [VerticalGroup(10f, true, nameof(woodTableCanvas), nameof(woodTableItemImage), nameof(woodTableItemNameLabel), nameof(woodTableItemProgressLabel))]
    Void woodTableGroup;
    [SerializeField]
    [HideProperty]
    Canvas woodTableCanvas;
    [SerializeField]
    [HideProperty]
    Image woodTableItemImage;
    [SerializeField]
    [HideProperty]
    TMP_Text woodTableItemNameLabel;
    [SerializeField]
    [HideProperty]
    TMP_Text woodTableItemProgressLabel;

    [SerializeField]
    [VerticalGroup(10f, true, nameof(craftTableCanvas), nameof(craftProductImage), nameof(craftProductNameLabel), nameof(craftMaterialListLabel))]
    Void craftTableGroup;
    [SerializeField]
    [HideProperty]
    Canvas craftTableCanvas;
    [SerializeField]
    [HideProperty]
    Image craftProductImage;
    [SerializeField]
    [HideProperty]
    TMP_Text craftProductNameLabel;
    [SerializeField]
    [HideProperty]
    TMP_Text craftMaterialListLabel;
    [SerializeField] MeshRenderer Modeling;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;

    [SerializeField]
    [VerticalGroup(10f, true, nameof(sawDustParticle), nameof(confettiParticle), nameof(animator))]
    Void effecterGroup;
    [SerializeField]
    [Required(), HideProperty]
    ParticleSystem sawDustParticle;
    [SerializeField]
    [Required(), HideProperty]
    ParticleSystem confettiParticle;
    Animator animator;
    TableEffecter tableEffecter;

    List<string> materialNames;

    IInteractableTool CurrentWorkingTool
    {
      get => this.currentWorkingTool;
      set
      {
        this.currentWorkingTool = value;
      }
    }
    protected override SmithingTool tool => this.woodTable;

    protected override ISmithingToolEffecter effecter => this.tableEffecter;

    protected override Transform materialPoint => this.materialPosition;

    IInteractableTool currentWorkingTool;

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (args.ItemToGive != null &&
        this.CurrentWorkingTool == this.woodTable)
      {
        bool canWoodTableTransfer = this.woodTable.CanTransferItem(args);
        if (canWoodTableTransfer)
        {
          return (true);
        }
        else
        {
          bool isCraftTableTansfer = this.craftTable.CanTransferItem(args);
          return (isCraftTableTansfer);
        }
      }
      else if (this.CurrentWorkingTool != null)
      {
        return (this.CurrentWorkingTool.CanTransferItem(args));
      }
      else
      {
        bool canWoodTableTransfer = this.woodTable.CanTransferItem(args);
        bool canCraftTableTransfer = this.craftTable.CanTransferItem(args);
        return (canWoodTableTransfer || canCraftTableTransfer);
      }
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      if (args.ItemToGive != null)
      {
        args.ItemToGive.transform.SetParent(this.transform);
        args.ItemToGive.transform.localPosition = Vector3.up;
        if (this.CurrentWorkingTool == this.woodTable &&
          !this.woodTable.CanTransferItem(args) &&
          this.woodTable.HoldingItem != null &&
          this.craftTable.CanTransferItem(args))
        {
          var result = this.MoveMaterialToCraftTable(args);
          this.OnTransfered?.Invoke(this, args, result);
          return (result);
        }
      }
      if (this.CurrentWorkingTool != null)
      {
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{nameof(Transfer)} result: {result}");
        if (result.IsDone)
        {
          this.CurrentWorkingTool = null;
        }
        if (this.CurrentWorkingTool == this.craftTable &&
          this.craftTable.HoldingMaterials.Count == 0)
        {
          this.CurrentWorkingTool = null;
        }
        this.OnTransfered?.Invoke(this, args, result);
        return (result);
      }
      else
      {
        if (this.woodTable.CanTransferItem(args))
        {
          this.CurrentWorkingTool = this.woodTable;
        }
        else if (this.craftTable.CanTransferItem(args))
        {
          this.CurrentWorkingTool = this.craftTable;
        }
        var result = this.CurrentWorkingTool.Transfer(args);
        Debug.Log($"{this.CurrentWorkingTool} {nameof(Transfer)} result: {result}");
        this.OnTransfered?.Invoke(this, args, result);
        return (result);
#if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(TableComponent)} is not able Transfer"));
#else
        return ( new ToolTransferResult {} );
#endif
      }
    }

    ToolTransferResult MoveMaterialToCraftTable(ToolTransferArgs args)
    {
      MaterialItem ItemToGive = args.ItemToGive;
      args.ItemToGive = null;
      var result = this.woodTable.Transfer(args);
      this.craftTable.Transfer(new ToolTransferArgs
      {
        ItemToGive = result.ReceivedItem as MaterialItem,
        PlayerNetworkId = args.PlayerNetworkId
      });
      args.ItemToGive = ItemToGive;
      this.CurrentWorkingTool = this.craftTable;
      return (this.craftTable.Transfer(args));
    }

    public override bool CanWork()
    {
      if (this.CurrentWorkingTool != null)
      {
        return (this.CurrentWorkingTool.CanWork());
      }
      return (false);
    }

    public override ToolWorkResult Work()
    {
      if (this.CurrentWorkingTool != null)
      {
        var result = this.CurrentWorkingTool.Work();
        if (this.CurrentWorkingTool == this.woodTable)
        {
          this.tableEffecter.TriggerWorkEffect();
        }
        else if (this.CurrentWorkingTool == this.craftTable)
        {
          this.animator.SetTrigger("Craft");
        }
        Debug.Log($"{nameof(Work)} result: {result}");
        this.OnWorked?.Invoke(this, result);
        return (result);
      }
      else
      {
#if UNITY_EDITOR
        throw new ApplicationException($"{nameof(TableComponent)} is not workable try ${nameof(CanWork)} first");
#else
        return (new ToolWorkResult {});
#endif
      }
    }

    void BeforeWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable)
      {
        return;
      }
      Debug.Log("BeforeInteract args");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterWoodTableInteract(SmithingTool tool)
    {
      if (tool != this.woodTable)
      {
        return;
      }
      Debug.Log("AfterInteract");
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
      Debug.Log($"currentWorkingTool: {this.currentWorkingTool}");
      if (tool.HoldingItem != null)
      {
        this.SetItemUI(tool.HoldingItem);
        if (tool.InteractionToTrigger == SmithingTool.InteractionType.Work)
        {
          this.highlighter.HighlightColor = this.interactColor;
        }
      }
      else
      {
        this.woodTableCanvas.enabled = false;
      }
    }

    void OnWoodTableTriggered(SmithingTool.InteractionType interactionType)
    {
      this.highlighter.HighlightColor = this.normalColor;
    }

    void SetItemUI(Item item)
    {
      this.woodTableItemImage.sprite = item.Data.Image;
      this.woodTableItemNameLabel.text = item.Data.Name;
      this.woodTableItemProgressLabel.text = $"Progress: {this.woodTable.Progress * 100}%";
      if (!this.woodTableCanvas.enabled)
      {
        this.woodTableCanvas.enabled = true;
      }

    }

    void OnCraftMaterialAdded(MaterialItem newMaterial)
    {
      if (!this.craftTableCanvas.enabled)
      {
        this.craftTableCanvas.enabled = true;
      }
      this.materialNames.Add(newMaterial.Data.Name);
      newMaterial.transform.localPosition = new Vector3(
        0, this.craftTable.HoldingMaterials.Count * 0.5f, 0);
      this.UpdateMaterialLabel();
    }

    void OnCraftMaterialRemoved(MaterialItem removedMaterial)
    {
      this.materialNames.Remove(removedMaterial.Data.Name);
      this.UpdateMaterialLabel();
      if (this.craftTable.HoldingMaterials.Count == 0 &&
        this.craftTableCanvas.enabled)
      {
        this.craftTableCanvas.enabled = false;
      }
    }

    void OnCraftProductCrafted(ProductItemData craftedProduct)
    {
      this.craftProductNameLabel.text = craftedProduct.Name;
      this.craftProductImage.sprite = craftedProduct.Image;
      this.tableEffecter.TriggerWorkEffect();
      this.audioLibrary.PlayRandomSound(
        soundName: "success",
        position: this.transform.position);
    }

    void OnCraftProductRemoved(ProductItemData removedProduct)
    {
      this.craftProductNameLabel.text = "";
      this.craftProductImage.sprite = null;
      this.craftTableCanvas.enabled = false;
    }

    void OnCraftableChanged()
    {
      if (this.craftTable.CraftableProduct != null)
      {
        this.craftProductNameLabel.text = $"craftable: {this.craftTable.CraftableProduct.Name}";
      }
      else
      {
        this.craftProductNameLabel.text = "";
      }
    }

    void UpdateMaterialLabel()
    {
      var builder = new StringBuilder("Materials: ");
      foreach (var name in this.materialNames)
      {
        builder.Append($"{name}, ");
      }
      this.craftMaterialListLabel.text = builder.ToString();
    }

    protected override void HandleNetworkTransfer(object[] args)
    {
      var dict = args[0] as Dictionary<string, object>;
      int playerNetworkId = (int)dict[ToolTransferArgs.PLAYER_NETWORK_ID_KEY];
      if (dict.TryGetValue(
          ToolTransferArgs.ITEM_ID_KEY, out object itemId) &&
        itemId != null)
      {
        if (this.NetworkSynchronizer.TryFindComponentFromNetworkId(
            networId: (int)itemId,
            out MaterialItem foundItem
            ))
        {
          this.Transfer(new ToolTransferArgs
          {
            ItemToGive = foundItem,
            PlayerNetworkId = playerNetworkId
          });
        }
#if UNITY_EDITOR
        else
        {
          Debug.LogError($"item not found for {args[0]}");
        }
#endif
      }
      else
      {
        //FIXME: Return item to player
        if (this.woodTable.HoldingItem != null)
        {
          this.woodTable.HoldingItem.transform.SetParent(null);
        }
        else if (this.craftTable.Product != null)
        {
          this.craftTable.Product.transform.SetParent(null);
        }
        else if (this.craftTable.HoldingMaterials.Count > 0)
        {
          this.craftTable.HoldingMaterials[this.craftTable.HoldingMaterials.Count - 1].transform.SetParent(null);
        }
        this.Transfer(new ToolTransferArgs
        {
          ItemToGive = null,
          PlayerNetworkId = playerNetworkId
        });
      }
    }

    protected override void Awake()
    {
      base.meshRenderer = Modeling;
      base.Awake();
      this.woodTable = new WoodTable(this.woodTableData);
      this.woodTable.BeforeInteract += this.BeforeWoodTableInteract;
      this.woodTable.AfterInteract += this.AfterWoodTableInteract;
      this.woodTable.OnInteractionTriggered += this.OnWoodTableTriggered;
      this.woodTableCanvas.enabled = false;
      this.craftTable = new CraftTable(
        data: this.craftTableData,
        productPoint: this.materialPoint);
      this.craftTable.OnMaterialAdded += this.OnCraftMaterialAdded;
      this.craftTable.OnMaterialRemoved += this.OnCraftMaterialRemoved;
      this.craftTable.OnProductCrafted += this.OnCraftProductCrafted;
      this.craftTable.OnProductRemoved += this.OnCraftProductRemoved;
      this.craftTable.OnCraftableChanged += this.OnCraftableChanged;
      this.tableEffecter = new TableEffecter(
        woodTable: this.woodTable,
        craftTable: this.craftTable,
        sawDustParticleSystem: this.sawDustParticle,
        confettiParticleSystem: this.confettiParticle,
        getCurrentTool: () => this.CurrentWorkingTool
        );
      this.materialNames = new();
      this.woodTableCanvas.enabled = false;
      this.craftTableCanvas.enabled = false;
      this.animator = this.GetComponent<Animator>();
    }

    protected override void HandleNetworkWork(object[] args)
    {
      // TODO: handle work result
      Debug.Log("HandleNetworkWork");
      var dict = args[0] as Dictionary<string, object>;
      foreach (var (key, value) in dict)
      {
        Debug.Log($"{key}: {value}");
      }
      this.Work();
      // TODO: handle work trigger
      if (this.CurrentWorkingTool == this.woodTable)
      {
      }
    }
  }
}
