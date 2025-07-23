using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  using Player = TestPlayer;
  using MaterialItem = TestMaterialItem;
  using MaterialType = TestMaterialType;

    public class Anvil : SmithingTool , IInteractable
    {

      public Anvil(SmithingToolData data): base(data)
      {
      }

      public IEnumerator Interact(Player player)
      {
        #if UNITY_EDITOR
        if (!this.IsInteractable(player)) {
          throw (
            new ApplicationException(
              "player try to interact but not interactable"));
        }
        #endif
        if (!(player.HoldingItem is MaterialItem materialItem)) {
          yield break;
        }
        //TODO:  재료 아이템을 도구로 이동하거나 숨기고 아이콘 표시
        this.HoldingItem = materialItem; 
      }

      public bool IsInteractable(Player player)
      {
        if (!this.IsFinished || this.HoldingItem != null) {
          return (false);
        }
        if (player.HoldingItem == null ||
          !(player.HoldingItem is MaterialItem materialItem) ||
          Array.IndexOf(this.AllowedMaterials, materialItem) == -1) {
          return (false);
        }
        return (false);
      }
    }
}
