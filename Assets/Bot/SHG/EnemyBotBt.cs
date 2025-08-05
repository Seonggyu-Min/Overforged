using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public class EnemyBotBt : BehaviourTree
  {
    public const string CURRENT_RECIPE_KEY = "CurrentRecipe";
    IBot bot;

    public EnemyBotBt(
      IBot bot,
      IList<BtNode> children = null
      ) : base(children)
    {
      this.bot = bot;
    }

    public static EnemyBotBt KeepCraftingProductBt(IBot bot)
    {
      BtNode getRecipe = new BtGetRecipeNode();
      BtNode checkRecipe = new BtCheckRecipeNode(bot);
      BtNode createProductNode = new BtCreateProductNode(bot);
      BtNode submitNode = new BtSubmitProductLeaf(bot);
      BtNode craftNode = new BtSequenceNode(
        children: new BtNode[] {
        getRecipe, createProductNode, checkRecipe, submitNode });
      BtNode repeatNode = new BtRepeaterNode(
        target: craftNode,
        condition: () => false);
      EnemyBotBt bt = new EnemyBotBt(
        bot, 
        children: new BtNode[] { repeatNode });
      return (bt);
    }

    public override NodeState Evaluate()
    {
      NodeState result = base.Evaluate();
      if (result == NodeState.Success && this.children.Count > 0) {
        Debug.Log($"done");
      }
      return (result);
    }
  }
}
