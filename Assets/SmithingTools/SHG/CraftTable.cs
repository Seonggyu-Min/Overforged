using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using Craft = TestCraft;

  public class CraftTable: SmithingTool
  {
    Craft[] craftList;
    public List<MaterialItem> HoldingMaterials { get; private set; }
    public ProductItem Product { get; private set; }
    public ProductItemData CraftableProduct { get; private set; }
    public override bool IsFinished => this.Product != null;
    protected override bool isPlayerMovable => true;
    protected override bool isRemamingTimeElapse => false;

    public Action<MaterialItem> OnMaterialAdded;
    public Action<MaterialItem> OnMaterialRemoved;
    public Action<ProductItemData> OnProductCrafted;
    public Action<ProductItemData> OnProductRemoved;
    public Action OnCraftableChanged;

    public CraftTable(CraftTableData data): base(data)
    {
      this.craftList = new Craft[data.CraftList.Length];
      for (int i = 0; i < craftList.Length; i++) {
        this.craftList[i] = new Craft(data.CraftList[i]); 
      }
      this.HoldingMaterials = new List<MaterialItem>();
    }

    public override bool CanTransferItem(ToolTransferArgs args)
    {
      if (this.Product != null) {
        return (args.ItemToGive == null);
      }
      if (args.ItemToGive != null) {
        return (this.IsCraftMaterial(
            args.ItemToGive.Data as MaterialItemData));
      }
      return (this.HoldingMaterials.Count > 0);
    }

    public override ToolTransferResult Transfer(ToolTransferArgs args)
    {
      this.BeforeInteract?.Invoke(this);
      if (this.Product != null) {
        ProductItem product = this.Product;
        this.Product = null;
        this.OnProductRemoved?.Invoke(product.Data as ProductItemData);
        return ( new ToolTransferResult { 
          ReceivedItem = product,
          IsDone = true
          });
      }
      if (args.ItemToGive != null) {
        this.HoldingMaterials.Add(args.ItemToGive); 
        this.OnMaterialAdded?.Invoke(args.ItemToGive);
        this.SetCraftableProduct();
        return (new ToolTransferResult {
            ReceivedItem = null,
            IsDone = false
          });
      }
      MaterialItem material = this.HoldingMaterials[this.HoldingMaterials.Count - 1];
      this.HoldingMaterials.RemoveAt(this.HoldingMaterials.Count - 1);
      this.OnMaterialRemoved?.Invoke(material);
      this.SetCraftableProduct();
      return (new ToolTransferResult { 
        ReceivedItem = material,
        IsDone = false
        });
    }

    void SetCraftableProduct()
    {
      bool wasCraftable = this.CraftableProduct != null;
      bool isCraftable = this.FindCraftable(out Craft craftable);
      this.CraftableProduct = isCraftable ?
        craftable.ProductItemData: null; 
      if (wasCraftable || isCraftable) {
        this.OnCraftableChanged?.Invoke();
      }
    }

    public override bool CanWork()
    {
      if (this.HoldingMaterials.Count > 0 &&
        this.FindCraftable(out Craft found)) {
        return (true);
      }
      return (false);
    }

    public override ToolWorkResult Work()
    {
      if (!this.FindCraftable(out Craft craft)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(Work)}is not valid try  {nameof(CanWork)} first"));
        #endif
        return (new ToolWorkResult {});
      }
      this.Product = craft.CreateProduct();
      foreach (var material in this.HoldingMaterials) {
        GameObject.Destroy(material.gameObject);
      }
      this.HoldingMaterials.Clear();
      this.CraftableProduct = null;
      this.OnCraftableChanged?.Invoke();
      return (new ToolWorkResult { 
          Trigger = this.OnTrigger,
          DurationToStay = 0.5f
        });
    }

    bool FindCraftable(out Craft found)
    {
      HashSet<MaterialItemData> currentMaterials = new ();
      foreach (var material in this.HoldingMaterials) {
        currentMaterials.Add(((Item)material).Data as MaterialItemData);
      }
      int foundIndex = Array.FindIndex(this.craftList, 
        craft => craft.Materials.SetEquals(currentMaterials));
      if (foundIndex != -1) {
        found = this.craftList[foundIndex];
        return (true);
      }
      found = null;
      return (false);
    }

    void OnTrigger()
    {
      this.OnProductCrafted?.Invoke(this.Product.Data as ProductItemData);
    }

    bool IsCraftMaterial(MaterialItemData materialData)
    {
      foreach (Craft craft in this.craftList) {
        if (craft.Materials.Contains(materialData)) {
          return (true);
        }
      }
      return (false);
    }
  }
}
