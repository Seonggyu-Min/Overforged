using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtCreateProductNode: BtSequenceNode
  {
    ProductRecipe recipe;
    IBot bot;
    TableComponent table;

    public BtCreateProductNode(
      Nullable<ProductRecipe> recipe,
      IBot bot,
      BtNode parent = null,
      IList<BtNode> children = null
      ): base(parent, children)
    {
      this.bot = bot;
      if (recipe != null) {
        this.Init(recipe.Value);
      }
    }

    public BtCreateProductNode Init(ProductRecipe recipe)
    {
      if (this.table == null) {
        if (this.bot.TryFindTool(SmithingTool.ToolType.WoodTable,
            out SmithingToolComponent found)) {
          this.table = found as TableComponent;
        }
      }
      if (this.table == null) {
        return (this);
      }
      this.recipe = recipe;
      int woodTablePartIndex = Array.FindIndex(
        this.recipe.Parts, this.IsWoodTablePart);
      if (woodTablePartIndex != -1) {
        this.AddChild(
          new BtCreatePartNode(
            bot: this.bot,
            part: this.recipe.Parts[woodTablePartIndex],
            takePart: false
            )
          );
      }
      for (int i = 0; i < this.recipe.Parts.Length; i++) {
        if (i == woodTablePartIndex) {
          continue;
        } 
        this.AddChild(
          new BtCreatePartNode(
            bot: this.bot,
            part: this.recipe.Parts[i],
            takePart: true));
        this.AddChild(
          new BtConditionalNode(
            condition: () => this.bot.IsHoldingHotMaterial(),
            trueNode: new BtQuenchingNode(
              bot: this.bot),
            falseNode: null));
        this.AddChild(
          new BtTransferItemLeaf(
            toGive: true,
            tool: this.table,
            transform: this.table.transform,
            bot: this.bot));
      }
      this.AddChild(
        new BtWorkLeaf(
          tool: this.table,
          transform: this.table.transform,
          bot: this.bot));
      this.AddChild(
        new BtTransferItemLeaf(
          toGive: false,
          tool: this.table,
          transform: this.table.transform,
          bot: this.bot));
      return (this);
    }

    bool IsWoodTablePart(Part part)
    {
      var partToolType = part.GetToolType();
      if (partToolType == null) {
        return (false);
      }
      return (partToolType.Value == SmithingTool.ToolType.WoodTable);
    }
  }
}
