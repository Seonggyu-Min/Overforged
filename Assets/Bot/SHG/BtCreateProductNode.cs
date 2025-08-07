using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtCreateProductNode: BtSequenceNode
  {
    Nullable<ProductRecipe> recipe;
    IBot bot;
    TableComponent table;

    public BtCreateProductNode(
      IBot bot,
      BtNode parent = null,
      IList<BtNode> children = null
      ): base(parent, children)
    {
      this.bot = bot;
    }

    public BtCreateProductNode Init(ProductRecipe recipe)
    {
      this.children.Clear();
      this.currentChildIndex = 0;
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
        recipe.Parts, this.IsWoodTablePart);
      if (woodTablePartIndex != -1) {
        this.AddChild(
          new BtCreatePartNode(
            bot: this.bot,
            part: recipe.Parts[woodTablePartIndex],
            takePart: false));
      }
      for (int i = 0; i < recipe.Parts.Length; i++) {
        if (i == woodTablePartIndex) {
          continue;
        } 
        this.AddChild(
          new BtCreatePartNode(
            bot: this.bot,
            part: recipe.Parts[i],
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

    public override NodeState Evaluate()
    {
      if (this.recipe == null) {
        if (this.TryFindData(
            EnemyBotBt.CURRENT_RECIPE_KEY, out object found) &&
          found is ProductRecipe recipe) {
          this.Init(recipe);
        }
      }
      return (base.Evaluate());
    }

    public override void Reset()
    {
      this.recipe = null;
      this.ClearData(EnemyBotBt.CURRENT_RECIPE_KEY);
    }
  }
}
