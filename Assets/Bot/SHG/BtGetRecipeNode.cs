using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class BtGetRecipeNode : BtNode
  {

    public BtGetRecipeNode(
      BtNode parent = null, 
      IList<BtNode> children = null): base(parent, children)
    {
    }

    public override NodeState Evaluate() 
    {
      if (this.TryFindData(
          EnemyBotBt.CURRENT_RECIPE_KEY, out object found) &&
        found is ProductRecipe recipe) {
        return (this.ReturnState(NodeState.Success));
      }
      if (BotContext.Instance.TryGetNextRecipe(
          out ProductRecipe nextRecipe)) {
        if (this.Parent != null) {
          this.Parent.SetData(EnemyBotBt.CURRENT_RECIPE_KEY, nextRecipe);
        }
        else {
          this.SetData(EnemyBotBt.CURRENT_RECIPE_KEY, nextRecipe);
        }
        return (this.ReturnState(NodeState.Success));
      }
      return (this.ReturnState(NodeState.Failure));
    }
  }
}
