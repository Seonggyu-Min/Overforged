using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtCheckRecipeNode : BtNode
  {
    IBot bot;

    public BtCheckRecipeNode(
      IBot bot,
      BtNode parent = null,
      IList<BtNode> children = null): base(parent, children)
    {
      this.bot = bot;
    }

    public override NodeState Evaluate() 
    {
      if (this.TryFindData(
          EnemyBotBt.CURRENT_RECIPE_KEY, out object found) &&
        found is ProductRecipe recipe) {
        if (BotContext.Instance.IsValidRecipe(recipe)) {
          return (this.ReturnState(NodeState.Success));
        }
        this.bot.PutDownItem();
        this.ClearData(EnemyBotBt.CURRENT_RECIPE_KEY);
        return (this.ReturnState(NodeState.Failure));
      }
      else {
        return (this.ReturnState(NodeState.Failure));
      }
    }
  }
}
